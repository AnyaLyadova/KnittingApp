import type { Draft, Point } from '../../types/Draft';
import { DraftService } from '../../services/DraftService'
import React, { useState, useEffect, useRef, useCallback, forwardRef, useImperativeHandle } from 'react';
import { DndContext, useDraggable } from '@dnd-kit/core';
import type { DragMoveEvent, DragStartEvent } from '@dnd-kit/core';
import type { LoopMap } from '../../types/Schema';
import { ConstructorService } from '../../services/ConstructorService';
import { ModelService } from '../../services/ModelService';



export interface DraftEditorRef {
    colorCell: (mIndex: number, nIndex: number, color: string) => void;
}

// Компонент одной точки (перетаскиваемый)
const DraggablePoint: React.FC<{
    id: string;
    x: number;
    y: number;
    isVisible: boolean;
    isSelected: boolean;
    isMoving: boolean;
    isDraggingActive: boolean;
    scale: number;
    offsetX: number;
    offsetY: number;
    canvasHeight: number;
    onPointClick?: (index: number, x: number, y: number, part: string) => void;
    pointIndex: number;
    part: string;
}> = ({
    id, x, y, isVisible, isSelected, isMoving, isDraggingActive, scale, offsetX, offsetY, canvasHeight,
    onPointClick, pointIndex, part
}) => {
        const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({ id });

        if (!isVisible) return null;

        const canvasX = x * scale + offsetX;
        const canvasY = canvasHeight - (y * scale + offsetY);

        const style = transform
            ? {
                transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
                left: canvasX,
                top: canvasY,
            }
            : {
                left: canvasX,
                top: canvasY,
            };

        let pointColor = '#3b82f6';
        if (isMoving) pointColor = '#ef4444';
        else if (isSelected) pointColor = '#22c55e';
        else if (isDragging || isDraggingActive) pointColor = '#ff6600';

        const handleClick = (e: React.MouseEvent) => {
            e.stopPropagation();
            if (onPointClick) {
                onPointClick(pointIndex, x, y, part);
            }
        };

        return (
            <div
                ref={setNodeRef}
                style={{
                    position: 'absolute',
                    width: 10,
                    height: 10,
                    backgroundColor: pointColor,
                    borderRadius: '50%',
                    cursor: 'grab',
                    touchAction: 'none',
                    border: `2px solid #ffffff`,
                    boxShadow: isSelected || isMoving ? '0 0 4px rgba(0,0,0,0.3)' : '0 0 2px rgba(0,0,0,0.2)',
                    zIndex: isSelected || isMoving ? 10 : 1,
                    transform: style.transform,
                    left: style.left,
                    top: style.top,
                }}
                onClick={handleClick}
                onMouseDown={handleClick}
                {...listeners}
                {...attributes}
            />
        );
    };

interface DraftEditorProps {
    draft: Draft | null;
    loopMap: LoopMap | null;
    width?: number;
    height?: number;
    onPointMove?: (index: number, oldX: number, oldY: number, newX: number, newY: number) => void;
    onPointClick?: (index: number, x: number, y: number, part: string) => void;
    onPointToggleVisibility?: (index: number, x: number, y: number, isNowVisible: boolean) => void;
    currentColor?: string;           // текущий выбранный цвет
    isEraserMode?: boolean;          // режим ластика
    onColorSchemeSaved?: (loopMap: LoopMap) => void;  // колбэк после сохранения
    onCellHover?: (mIndex: number, nIndex: number) => void;
    onGridMetricsChange?: (cellWidthPx: number, cellHeightPx: number) => void;
    isDraggingSchema?: boolean;
    skipNextCellClick?: boolean;
    onLoopMapUpdate?: (loopMap: LoopMap) => void;
    draftType: string;
    modelId?: string;
}

//export const DraftEditor: React.FC<DraftEditorProps> = ({
export const DraftEditor = forwardRef<DraftEditorRef, DraftEditorProps>(({
    draft,
    loopMap,
    width = 800,
    height = 600,
    onPointMove,
   // onPointClick,
    onPointToggleVisibility,
    currentColor = '#007aff',  
    isEraserMode = false,  
    onColorSchemeSaved,
    onCellHover,
    onGridMetricsChange,
    isDraggingSchema,
    skipNextCellClick,
    onLoopMapUpdate,
    draftType,
    modelId,
},ref) => {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const [points, setPoints] = useState<Point[]>([]);
    const [activePointId, setActivePointId] = useState<string | null>(null);
    const dragStartPointRef = useRef<{ x: number; y: number; index: number } | null>(null);

    const [selectedPoints, setSelectedPoints] = useState<number[]>([]);
    const [movingPointIndex, setMovingPointIndex] = useState<number | null>(null);
    const [selectionMode, setSelectionMode] = useState<'idle' | 'selectingAnchors' | 'selectingMoving'>('idle');
    const [phantomX, setPhantomX] = useState<number | null>(null);
    const [phantomY, setPhantomY] = useState<number | null>(null);
    const [statusMessage, setStatusMessage] = useState<string>('');

    const [loopMapState, setLoopMap] = useState<LoopMap | null>(loopMap);

 

    const [transformParams, setTransformParams] = useState<{
        scale: number;
        offsetX: number;
        offsetY: number;
    }>({ scale: 1, offsetX: 0, offsetY: 0 });

    const realCellWidth = loopMap?.loopWidth ?? 20;
    const realCellHeight = loopMap?.loopHeight ?? 25;

    // Синхронизация с пропсом draft
    useEffect(() => {
        if (draft?.draft) {
            setPoints(draft.draft);
        }

    }, [draft]);

    useEffect(() => {
        if (loopMap) {
            setLoopMap(loopMap);
        }
    }, [loopMap]);



    // В компоненте, в обработчике движения мыши на canvas
    const handleCanvasMouseMove = useCallback((e: React.MouseEvent<HTMLCanvasElement>) => {
        if (!loopMapState || !loopMapState.loopMap) return;

        // Вычисляем индексы ячейки под курсором
        const rect = canvasRef.current?.getBoundingClientRect();
        if (!rect) return;

        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;

        const { scale, offsetX, offsetY } = transformParams;
        const cellWidthPx = realCellWidth * scale;
        const cellHeightPx = realCellHeight * scale;

        const originX = offsetX;
        const originY = canvasRef.current!.height - offsetY;
        const startGridX = originX - (loopMapState.nullN ?? 0) * cellWidthPx;
        const startGridY = originY - (loopMapState.nullM ?? 0) * cellHeightPx;

        const nIndex = Math.floor((mouseX - startGridX) / cellWidthPx);
        const mIndex = Math.floor((mouseY - startGridY) / cellHeightPx);

        if (mIndex >= 0 && mIndex < loopMapState.m && nIndex >= 0 && nIndex < loopMapState.n) {
            onCellHover?.(mIndex, nIndex);
        }
    }, [loopMapState, transformParams, realCellWidth, realCellHeight, onCellHover]);


    // ==================== СОСТОЯНИЯ ДЛЯ ЦВЕТА ====================
    const [pendingColorChanges, setPendingColorChanges] = useState<{
        mIndexes: number[];
        nIndexes: number[];
        colors: string[];
    }>({ mIndexes: [], nIndexes: [], colors: [] });


    // ==================== ОБРАБОТЧИК КЛИКА ПО ЯЧЕЙКЕ ====================
    /*const handleCellClick = useCallback((mIndex: number, nIndex: number) => {

        if (isDraggingSchema) {  //ничего не делаем, если размещаем схему

            return;
        }
        if (!loopMap || !loopMap.loopMap) return;


        // Цвет, который будем устанавливать
        const colorValue = isEraserMode ? '#ffffff' : (currentColor ?? '#007aff');

        // 1. Создаём копию и обновляем цвет
        const newLoopMap = JSON.parse(JSON.stringify(loopMapState));
        if (newLoopMap.loopMap?.[mIndex]?.[nIndex]) {
            newLoopMap.loopMap[mIndex][nIndex] = {
                ...newLoopMap.loopMap[mIndex][nIndex],
                color: colorValue
            };

            setLoopMap(newLoopMap);
        }

        // 2. Сохраняем изменение в список ожидающих отправки
        setPendingColorChanges(prev => {
            const existingIndex = prev.mIndexes.findIndex(
                (m, idx) => m === mIndex && prev.nIndexes[idx] === nIndex
            );

            if (existingIndex !== -1) {
                const newColors = [...prev.colors];
                newColors[existingIndex] = colorValue;
                return {
                    mIndexes: [...prev.mIndexes],
                    nIndexes: [...prev.nIndexes],
                    colors: newColors
                };
            } else {
                return {
                    mIndexes: [...prev.mIndexes, mIndex],
                    nIndexes: [...prev.nIndexes, nIndex],
                    colors: [...prev.colors, colorValue]
                };
            }
        });
    }, [loopMapState, currentColor, isEraserMode]);*/


    const handleCellClick = useCallback((mIndex: number, nIndex: number, schemaColor?: string) => {

        // Если перетаскивается схема и это не вызов из applySchemaToGrid - пропускаем
        if (isDraggingSchema && schemaColor === undefined) {
            return;
        }
        if (!loopMapState || !loopMapState.loopMap) return;

        // Используем schemaColor если передан, иначе текущий выбранный цвет
        const colorValue = schemaColor ?? (isEraserMode ? '#ffffff' : (currentColor ?? '#007aff'));

        // 1. Создаём копию и обновляем цвет
        const newLoopMap = JSON.parse(JSON.stringify(loopMapState));
        if (newLoopMap.loopMap?.[mIndex]?.[nIndex]) {
            newLoopMap.loopMap[mIndex][nIndex] = {
                ...newLoopMap.loopMap[mIndex][nIndex],
                color: colorValue
            };

            setLoopMap(newLoopMap);
        }

        // 2. Сохраняем изменение в список ожидающих отправки
        setPendingColorChanges(prev => {
            const existingIndex = prev.mIndexes.findIndex(
                (m, idx) => m === mIndex && prev.nIndexes[idx] === nIndex
            );

            if (existingIndex !== -1) {
                const newColors = [...prev.colors];
                newColors[existingIndex] = colorValue;
                return {
                    mIndexes: [...prev.mIndexes],
                    nIndexes: [...prev.nIndexes],
                    colors: newColors
                };
            } else {
                return {
                    mIndexes: [...prev.mIndexes, mIndex],
                    nIndexes: [...prev.nIndexes, nIndex],
                    colors: [...prev.colors, colorValue]
                };
            }
        });
    }, [loopMapState, currentColor, isEraserMode, isDraggingSchema]);



    // Функция для скачивания PNG
    const downloadAsPNG = useCallback(() => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        // Создаем ссылку на изображение
        const link = document.createElement('a');
        link.download = `draft-${draftType}-${Date.now()}.png`;
        link.href = canvas.toDataURL('image/png');
        link.click();
    }, [draftType]);

    // Функция для скачивания PDF
    const downloadAsPDF = useCallback(async () => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        // Импортируем jsPDF
        const { jsPDF } = await import('jspdf');

        // Получаем данные canvas
        const imgData = canvas.toDataURL('image/png');

        // Создаем PDF с размерами canvas
        const pdf = new jsPDF({
            orientation: canvas.width > canvas.height ? 'landscape' : 'portrait',
            unit: 'px',
            format: [canvas.width, canvas.height]
        });

        pdf.addImage(imgData, 'PNG', 0, 0, canvas.width, canvas.height);
        pdf.save(`draft-${draftType}-${Date.now()}.pdf`);
    }, [draftType]);

    // ==================== СОХРАНЕНИЕ ЦВЕТОВОЙ СХЕМЫ ====================
    const handleSaveColorScheme = useCallback(async () => {
        // если нет изменений — не отправляем
        if (pendingColorChanges.mIndexes.length === 0) {
            setStatusMessage('Нет изменений для сохранения');
            setTimeout(() => setStatusMessage(''), 2000);
            return;
        }

        try {
            setStatusMessage('Сохранение цветовой схемы...');

            const newLoopMap = await ConstructorService.colorLoopMap(
                modelId,
                pendingColorChanges.mIndexes,
                pendingColorChanges.nIndexes,
                pendingColorChanges.colors,
                draftType
            );

            if (newLoopMap) {
                // Обновляем локальный loopMap 
              //  setLoopMap(newLoopMap);
                setStatusMessage('Цветовая схема сохранена!');
            }

            // Очищаем список ожидающих изменений
            setPendingColorChanges({ mIndexes: [], nIndexes: [], colors: [] });

            // Уведомляем родителя
            onColorSchemeSaved?.(newLoopMap);

        } catch (error) {
            console.error('Ошибка при сохранении цветовой схемы:', error);
            setStatusMessage('Ошибка при сохранении цветовой схемы');
        } finally {
            setTimeout(() => setStatusMessage(''), 2000);
        }
    }, [pendingColorChanges, onColorSchemeSaved, isDraggingSchema]);



    // ==================== ОБРАБОТЧИК КЛИКА ПО ЯЧЕЙКЕ НА CANVAS ====================
    const handleCanvasCellClick = useCallback((e: React.MouseEvent<HTMLCanvasElement>) => {

        if (isDraggingSchema) {
            return;
        }
        if (skipNextCellClick) return;

        if (!loopMapState || !loopMapState.loopMap || loopMapState.m === 0 || loopMapState.n === 0) return;

        const canvas = canvasRef.current;
        if (!canvas) return;

        const rect = canvas.getBoundingClientRect();
        const mouseX = e.clientX - rect.left;
        const mouseY = e.clientY - rect.top;

        const { scale, offsetX, offsetY } = transformParams;
        const cellWidthPx = realCellWidth * scale;
        const cellHeightPx = realCellHeight * scale;

        // Находим начало сетки (как в drawGridAndCells)
        const originX = offsetX;
        const originY = canvas.height - offsetY;
        const startGridX = originX - (loopMapState.nullN ?? 0) * cellWidthPx;
        const startGridY = originY - (loopMapState.nullM ?? 0) * cellHeightPx;

        // Вычисляем индекс ячейки, по которой кликнули
        const nIndex = Math.floor((mouseX - startGridX) / cellWidthPx);
        const mIndex = Math.floor((mouseY - startGridY) / cellHeightPx);

        // Проверяем, что клик в пределах матрицы
        if (mIndex >= 0 && mIndex < loopMapState.m && nIndex >= 0 && nIndex < loopMapState.n) {
            handleCellClick(mIndex, nIndex);
        }

    }, [loopMapState, transformParams, realCellWidth, realCellHeight, handleCellClick, isDraggingSchema, skipNextCellClick]);

    // Вычисление параметров масштабирования
    const calculateTransformParams = useCallback((pointsData: Point[], w: number, h: number) => {
        if (!pointsData || pointsData.length === 0) {
            return { scale: 1, offsetX: 0, offsetY: 0 };
        }

        let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
        for (const point of pointsData) {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.y > maxY) maxY = point.y;
        }

        const padding = 40;
        const rangeX = maxX - minX;
        const rangeY = maxY - minY;

        if (rangeX === 0 || rangeY === 0) {
            return { scale: 1, offsetX: 0, offsetY: 0 };
        }

        const scaleX = (w - padding * 2) / rangeX;
        const scaleY = (h - padding * 2) / rangeY;
        const scale = Math.min(scaleX, scaleY);
        const offsetX = padding - minX * scale;
        const offsetY = padding - minY * scale;

        return { scale, offsetX, offsetY };
    }, []);

    // Обновляем параметры масштабирования
    useEffect(() => {
        const newParams = calculateTransformParams(points, width, height);
        setTransformParams(newParams);
        const cellWidthPx = realCellWidth * transformParams.scale;
        const cellHeightPx = realCellHeight * transformParams.scale;
        onGridMetricsChange?.(cellWidthPx, cellHeightPx);
    }, [points, width, height, calculateTransformParams, onGridMetricsChange, ]);

    // ==================== ОТРИСОВКА СЕТКИ И ЯЧЕЕК ====================
    const drawGridAndCells = useCallback((
        ctx: CanvasRenderingContext2D,
        canvasWidth: number,
        canvasHeight: number,
        currentLoopMap: LoopMap,
        scale: number,
        offsetX: number,
        offsetY: number,
        cellWidthPx: number,
        cellHeightPx: number
    ) => {
        const { m, n, nullM = 0, nullN = 0, loopMap: matrix } = currentLoopMap;

        if (m === 0 || n === 0 || !matrix) return;

        // Находим позицию точки (0,0) чертежа на canvas
        const originX = offsetX;  // т.к. points[0].x = 0
        const originY = canvasHeight - offsetY;  // т.к. points[0].y = 0

        // Начало сетки (верхний левый угол ячейки [0][0])
        const startGridX = originX - nullN * cellWidthPx;
        const startGridY = originY - nullM * cellHeightPx;

        // Рисуем цветные ячейки
        for (let i = 0; i < m; i++) {
            for (let j = 0; j < n; j++) {
                const loop = matrix[i]?.[j];

                // Определяем цвет ячейки
                let fillColor = '#c0c0c0'; // светло-серый по умолчанию

                if (loop && loop.type !== 'none') {
                    if (loop.color && loop.color !== 'none') {
                        fillColor = loop.color;
                    } else {
                        fillColor = '#FFFFFF'; // белый
                    }
                } else {
                    fillColor = '#c0c0c0'; // светло-серый для null или type=none
                }

                // Координаты ячейки на canvas


                const cellX = startGridX + j * cellWidthPx - cellWidthPx / 2;
                const cellY = startGridY + i * cellHeightPx - cellHeightPx / 2;

                // Закрашиваем только видимую область
              //  if (cellX + cellWidthPx >= 0 && cellX <= canvasWidth &&
                   // cellY + cellHeightPx >= 0 && cellY <= canvasHeight) {
                    ctx.fillStyle = fillColor;
                    ctx.fillRect(cellX, cellY, cellWidthPx, cellHeightPx);
              //  }
            }
        }

        // Рисуем линии сетки поверх ячеек
        ctx.beginPath();
        ctx.strokeStyle = '#e0e0e0';
        ctx.lineWidth = 1;

        // Вертикальные линии (n+1 штук)
       /* for (let j = 0; j <= n; j++) {
            const x = startGridX + j * cellWidthPx;*/
        for (let j = 0; j <= n; j++) {
            const x = startGridX + j * cellWidthPx - cellWidthPx / 2;
            if (x >= 0 && x <= canvasWidth) {
                ctx.beginPath();
                ctx.moveTo(x, 0);
                ctx.lineTo(x, canvasHeight);
                ctx.stroke();
            }
        }

        // Горизонтальные линии (m+1 штук)
        /*for (let i = 0; i <= m; i++) {
            const y = startGridY + i * cellHeightPx;*/
        for (let i = 0; i <= m; i++) {
            const y = startGridY + i * cellHeightPx - cellHeightPx / 2;
            if (y >= 0 && y <= canvasHeight) {
                ctx.beginPath();
                ctx.moveTo(0, y);
                ctx.lineTo(canvasWidth, y);
                ctx.stroke();
            }
        }
    }, []);

    // ==================== ОТРИСОВКА CANVAS ====================
    useEffect(() => {


        const canvas = canvasRef.current;
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        ctx.clearRect(0, 0, width, height);
        ctx.fillStyle = '#f9f9f9';
        ctx.fillRect(0, 0, width, height);

        if (!draft || !draft.draft || draft.draft.length === 0) {
            ctx.fillStyle = '#999';
            ctx.font = '24px sans-serif';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText('Выберите модель', width / 2, height / 2);
            return;
        }

        const { scale, offsetX, offsetY } = transformParams;
        const cellWidthPx = realCellWidth * scale;
        const cellHeightPx = realCellHeight * scale;


        // Рисуем сетку и ячейки на основе loopMap (если есть)
        if (loopMapState && loopMapState.m > 0 && loopMapState.n > 0 && points.length > 0) {
            drawGridAndCells(
                ctx, width, height, loopMapState,
                scale, offsetX, offsetY,
                cellWidthPx, cellHeightPx
            );
        }

        // ==================== ШТРИХОВЫЕ ЛИНИИ ====================
        if (selectedPoints.length === 2 && movingPointIndex !== null && phantomX !== null && phantomY !== null) {
            const [leftIndex, rightIndex] = selectedPoints;
            const leftPoint = points[leftIndex];
            const rightPoint = points[rightIndex];

            if (leftPoint && rightPoint) {
                const leftX = leftPoint.x * scale + offsetX;
                const leftY = canvas.height - (leftPoint.y * scale + offsetY);
                const rightX = rightPoint.x * scale + offsetX;
                const rightY = canvas.height - (rightPoint.y * scale + offsetY);
                const phantomCanvasX = phantomX * scale + offsetX;
                const phantomCanvasY = canvas.height - (phantomY * scale + offsetY);

                ctx.save();
                ctx.strokeStyle = '#a855f7';
                ctx.lineWidth = 2;
                ctx.setLineDash([5, 5]);

                ctx.beginPath();
                ctx.moveTo(leftX, leftY);
                ctx.lineTo(phantomCanvasX, phantomCanvasY);
                ctx.stroke();

                ctx.beginPath();
                ctx.moveTo(phantomCanvasX, phantomCanvasY);
                ctx.lineTo(rightX, rightY);
                ctx.stroke();

                ctx.restore();
            }
        }

        // ==================== ОСНОВНЫЕ ЛИНИИ ====================
        if (points.length > 0) {
            ctx.beginPath();
            ctx.strokeStyle = '#000000';
            ctx.lineWidth = 2;

            for (let i = 0; i < points.length - 1; i++) {
                const isPhantomSegment = selectedPoints.length === 2 && movingPointIndex !== null &&
                    ((i === selectedPoints[0] && i + 1 === movingPointIndex) ||
                        (i === movingPointIndex && i + 1 === selectedPoints[1]));

                if (isPhantomSegment && phantomX !== null) {
                    continue;
                }

                const p1 = points[i];
                const p2 = points[i + 1];

                const x1 = p1.x * scale + offsetX;
                const y1 = canvas.height - (p1.y * scale + offsetY);
                const x2 = p2.x * scale + offsetX;
                const y2 = canvas.height - (p2.y * scale + offsetY);

                ctx.beginPath();
                ctx.moveTo(x1, y1);
                ctx.lineTo(x2, y2);
                ctx.stroke();
            }
        }
    }, [points, width, height, draft, transformParams, realCellWidth, realCellHeight, selectedPoints, movingPointIndex, phantomX, phantomY, loopMapState, drawGridAndCells]);

    // ==================== ОБРАБОТЧИК ВЫБОРА ТОЧЕК ====================
    const isPointBetweenAnchors = useCallback((pointIndex: number, anchorIndices: number[]): boolean => {
        if (anchorIndices.length !== 2) return false;
        const [idx1, idx2] = anchorIndices;
        const startIdx = Math.min(idx1, idx2);
        const endIdx = Math.max(idx1, idx2);
        return pointIndex > startIdx && pointIndex < endIdx;
    }, []);

    const handlePointSelection = (index: number/*, x: number, y: number, part: string*/) => {
        if (movingPointIndex !== null) {
            setSelectedPoints([]);
            setMovingPointIndex(null);
            setPhantomX(null);
            setPhantomY(null);
            setSelectionMode('idle');
            setStatusMessage('');
            return;
        }

        if (selectedPoints.length < 2) {
            if (selectedPoints.includes(index)) {
                setSelectedPoints([]);
                setSelectionMode('idle');
                setStatusMessage('');
                return;
            }

            const newSelected = [...selectedPoints, index];
            setSelectedPoints(newSelected);

            if (newSelected.length === 1) {
                setStatusMessage('Выберите вторую опорную точку');
            } else if (newSelected.length === 2) {
                setStatusMessage('Выберите точку для перемещения');
                setSelectionMode('selectingMoving');
            }
            return;
        }

        if (selectedPoints.length === 2) {
            if (isPointBetweenAnchors(index, selectedPoints)) {
                setMovingPointIndex(index);
                setSelectionMode('idle');
                setStatusMessage('Перемещайте точку');
            } else {
                setStatusMessage('Ошибка: выберите точку между двумя опорными!');
                setTimeout(() => {
                    setStatusMessage('Выберите точку для перемещения');
                }, 2000);
            }
            return;
        }
    };

    // ==================== ОБРАБОТЧИКИ ПЕРЕТАСКИВАНИЯ ====================
    const handleDragStart = useCallback((event: DragStartEvent) => {
        const { active } = event;
        const activeId = String(active.id);
        const index = parseInt(activeId.split('-')[1], 10);
        const point = points[index];

        dragStartPointRef.current = {
            x: point.x,
            y: point.y,
            index: index
        };

        if (movingPointIndex === index && selectedPoints.length === 2) {
            setActivePointId(activeId);
        } else {
            if (movingPointIndex !== null) {
                setStatusMessage('Сначала завершите текущее перемещение (нажмите Escape)');
            } else if (selectionMode !== 'idle') {
                setStatusMessage('Сначала завершите выбор (нажмите Escape)');
            }
            return;
        }
    }, [movingPointIndex, selectedPoints, points, selectionMode]);

    const handleDragMove = useCallback((event: DragMoveEvent) => {
        if (movingPointIndex === null || selectedPoints.length !== 2) return;
        if (!dragStartPointRef.current) return;

        if (!loopMap || !loopMap.m || !loopMap.n) {
            setStatusMessage('Ошибка: нет данных сетки');
            return;
        }

        const { delta } = event;
        const startX = dragStartPointRef.current.x;
        const startY = dragStartPointRef.current.y;

        const deltaX = delta.x / transformParams.scale;
        const deltaY = -delta.y / transformParams.scale;

        let newX = startX + deltaX;
        let newY = startY + deltaY;

        // Привязка к сетке
        const stepX = realCellWidth;
        const stepY = realCellHeight;
        newX = Math.round(newX / stepX) * stepX;
        newY = Math.round(newY / stepY) * stepY;

        // Проверка границ с учетом перевернутой Y
        const { nullM = 0, nullN = 0, m, n } = loopMap;

        const newMIndex = nullM - Math.round(newY / stepY); 
        const newNIndex = nullN + Math.round(newX / stepX);


        if (newMIndex < 0 || newMIndex >= m || newNIndex < 0 || newNIndex >= n) {
            setStatusMessage('Точка в недопустимом диапазоне');
            return;
        }

        setStatusMessage('');
        setPhantomX(newX);
        setPhantomY(newY);
    }, [movingPointIndex, selectedPoints, transformParams.scale, realCellWidth, realCellHeight, loopMap]);

    const handleDragEnd = useCallback(async () => {
        if (movingPointIndex === null || selectedPoints.length !== 2 || phantomX === null || phantomY === null) {
            setPhantomX(null);
            setPhantomY(null);
            dragStartPointRef.current = null;
            return;
        }

        const originalPoint = points[movingPointIndex];
        const newX = phantomX;
        const newY = phantomY;
        const [leftIndex, rightIndex] = selectedPoints;
        const leftPoint = points[leftIndex];
        const rightPoint = points[rightIndex];

        if (!originalPoint || !leftPoint || !rightPoint) return;

        const leftAnchor = leftPoint.x < rightPoint.x ? leftPoint : rightPoint;
        const rightAnchor = leftPoint.x < rightPoint.x ? rightPoint : leftPoint;

        try {
            setStatusMessage('Отправка изменений на сервер...');
            console.log("Original point: " + originalPoint);
            console.log("new x: " + newX + " new y: " + newY);
            const newDraft = await DraftService.movePoint(
                modelId,
                originalPoint,
                newX,
                newY,
                leftAnchor,
                rightAnchor,
                draftType
            );

            if (newDraft && newDraft.draft) {
                setPoints(newDraft.draft);

                const newLoopMap = await ModelService.getLoopMap(modelId, draftType);
                if (newLoopMap) {
                    setLoopMap(newLoopMap);
                    onLoopMapUpdate?.(newLoopMap);

                    // Принудительная перерисовка
                    setTransformParams(prev => ({ ...prev }));
                }

                setStatusMessage('Точка перемещена!');
            }

            onPointMove?.(movingPointIndex, originalPoint.x, originalPoint.y, phantomX, phantomY);

        } catch (error) {
            console.error('Ошибка при перемещении точки:', error);
            setStatusMessage('Ошибка при перемещении точки');
        } finally {
            setSelectedPoints([]);
            setMovingPointIndex(null);
            setPhantomX(null);
            setPhantomY(null);
            setActivePointId(null);
            dragStartPointRef.current = null;
            setTimeout(() => setStatusMessage(''), 2000);
        }
    }, [movingPointIndex, selectedPoints, phantomX, phantomY, points, onPointMove, onLoopMapUpdate]);

    const cancelMovement = useCallback(() => {
        if (movingPointIndex !== null || selectedPoints.length > 0) {
            setSelectedPoints([]);
            setMovingPointIndex(null);
            setPhantomX(null);
            setPhantomY(null);
            setActivePointId(null);
            setSelectionMode('idle');
            dragStartPointRef.current = null;
            setTimeout(() => setStatusMessage(''), 2000);
        }
    }, [movingPointIndex, selectedPoints]);

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') {
                cancelMovement();
            }
        };
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [cancelMovement]);

    const findPointUnderCursor = useCallback((clientX: number, clientY: number): number | null => {
        const canvas = canvasRef.current;
        if (!canvas) return null;

        const rect = canvas.getBoundingClientRect();
        const mouseX = clientX - rect.left;
        const mouseY = clientY - rect.top;
        const { scale, offsetX, offsetY } = transformParams;

        for (let i = 0; i < points.length; i++) {
            const point = points[i];
            if (!point.visible) continue;

            const canvasX = point.x * scale + offsetX;
            const canvasY = canvas.height - (point.y * scale + offsetY);
            const dx = canvasX - mouseX;
            const dy = canvasY - mouseY;
            const dist = Math.sqrt(dx * dx + dy * dy);

            if (dist < 10) return i;
        }
        return null;
    }, [points, transformParams]);

    const findAnyPointUnderCursor = useCallback((clientX: number, clientY: number): number | null => {
        const canvas = canvasRef.current;
        if (!canvas) return null;

        const rect = canvas.getBoundingClientRect();
        const mouseX = clientX - rect.left;
        const mouseY = clientY - rect.top;
        const { scale, offsetX, offsetY } = transformParams;

        for (let i = 0; i < points.length; i++) {
            const point = points[i];
            const canvasX = point.x * scale + offsetX;
            const canvasY = canvas.height - (point.y * scale + offsetY);
            const dx = canvasX - mouseX;
            const dy = canvasY - mouseY;
            const dist = Math.sqrt(dx * dx + dy * dy);

            if (dist < 10) return i;
        }
        return null;
    }, [points, transformParams]);

    const handleCanvasDoubleClick = (e: React.MouseEvent<HTMLCanvasElement>) => {
        const pointIndex = findAnyPointUnderCursor(e.clientX, e.clientY);

        if (pointIndex !== null) {
            const point = points[pointIndex];
            const newPoints = [...points];
            newPoints[pointIndex] = {
                ...point,
                visible: !point.visible
            };
            setPoints(newPoints);

            if (onPointToggleVisibility) {
                onPointToggleVisibility(pointIndex, point.x, point.y, !point.visible);
            }
        }
    };

    const handleCanvasClick = (e: React.MouseEvent<HTMLCanvasElement>) => {

        // Если перетаскивается схема - игнорируем клики по точкам
        if (isDraggingSchema) {
            return;
        }

        const pointIndex = findPointUnderCursor(e.clientX, e.clientY);

        if (pointIndex !== null) {
            handlePointSelection(pointIndex/*, points[pointIndex].x, points[pointIndex].y, points[pointIndex].part || ''*/);
        } else {
            if (movingPointIndex !== null || selectedPoints.length > 0 || selectionMode !== 'idle') {
                cancelMovement();
            }
        }
    };

    const renderStatusBar = () => {
        if (statusMessage) {
            return (
                <div style={{
                    position: 'absolute',
                    top: -30,
                    left: 0,
                    right: 0,
                    textAlign: 'center',
                    fontSize: '12px',
                    color: statusMessage.includes('Ошибка') || statusMessage.includes('недопустимом') ? '#ef4444' : '#666',
                    backgroundColor: '#f9f9f9',
                    padding: '4px',
                    borderRadius: '4px',
                    border: '1px solid #e0e0e0',
                }}>
                    {statusMessage}
                </div>
            );
        }
        return null;
    };


    useImperativeHandle(ref, () => ({
        colorCell: (mIndex: number, nIndex: number, color: string) => {
            handleCellClick(mIndex, nIndex, color);
        }
    }));


    if (!draft) {
        return (
            <div style={{ width, height, display: 'flex', alignItems: 'center', justifyContent: 'center', border: '1px solid #ccc', borderRadius: '8px', backgroundColor: '#f9f9f9' }}>
                Выберите модель
            </div>
        );
    }

    const { scale, offsetX, offsetY } = transformParams;



    

   
    

    return (
        <div style={{ position: 'relative', width, height, border: '1px solid #ccc', borderRadius: '8px' }}>
            {renderStatusBar()}

            <div style={{ position: 'absolute', top: 10, right: 10, display: 'flex', gap: '10px', zIndex: 30 }}>
                <button onClick={downloadAsPNG}>Скачать PNG</button>
                <button onClick={downloadAsPDF}>Скачать PDF</button>
                <button onClick={handleSaveColorScheme}>Сохранить цветовую схему</button>
            </div>

            <canvas
                ref={canvasRef}
                width={width}
                height={height}
                style={{ position: 'absolute',top: 0, left: 0, borderRadius: '8px' }}
                onDoubleClick={handleCanvasDoubleClick}
               // onClick={handleCanvasClick}
                onClick={(e) => {
                    handleCanvasClick(e);           // существующий обработчик для точек
                    handleCanvasCellClick(e);       // новый обработчик для ячеек
                }}
                onMouseMove={handleCanvasMouseMove}
            />

            

            <DndContext onDragMove={handleDragMove} onDragEnd={handleDragEnd} onDragStart={handleDragStart}>
                {points.map((point, idx) => (
                    <DraggablePoint
                        key={`point-${idx}`}
                        id={`point-${idx}`}
                        x={point.x}
                        y={point.y}
                        isVisible={point.visible}
                        isSelected={selectedPoints.includes(idx)}
                        isMoving={movingPointIndex === idx}
                        isDraggingActive={activePointId === `point-${idx}`}
                        scale={scale}
                        offsetX={offsetX}
                        offsetY={offsetY}
                        canvasHeight={height}
                        onPointClick={handlePointSelection}
                        pointIndex={idx}
                        part={point.part || ''}
                    />
                ))}
            </DndContext>
        </div>
    );
});