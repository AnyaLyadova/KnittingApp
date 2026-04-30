import type { Draft, Point } from '../../types/Draft';
import { DraftService } from '../../services/DraftService'
import React, { useState, useEffect, useRef, useCallback } from 'react';
import { DndContext, useDraggable } from '@dnd-kit/core';
import type { DragMoveEvent, DragStartEvent, DragEndEvent } from '@dnd-kit/core';
import type { LoopMap } from '../../types/Schema';
//import { createSnapModifier } from '@dnd-kit/modifiers';


// Компонент одной точки (перетаскиваемый)
const DraggablePoint: React.FC<{
    id: string;
    x: number;
    y: number;
    isVisible: boolean;
    isSelected: boolean;           // для выбранных опорных точек
    isMoving: boolean;             // для перемещаемой точки
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

        // Вычисляем позицию на canvas
        const canvasX = x * scale + offsetX;
        const canvasY = canvasHeight - (y * scale + offsetY);

        // Применяем трансформацию от dnd-kit (она в пикселях)
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

    let pointColor = '#3b82f6'; // синий (обычная)
    if (isMoving) pointColor = '#ef4444'; // красный (перемещаемая)
    else if (isSelected) pointColor = '#22c55e'; // зелёный (опорная)
    else if (isDragging || isDraggingActive) pointColor = '#ff6600'; // оранжевый (перетаскивается)

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
                    border: `2px solid ${isSelected || isMoving ? '#ffffff' : '#ffffff'}`,
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

interface PatternEditorProps {
    draft: Draft | null;
    loopMap: LoopMap | null;
    width?: number;
    height?: number;
    //cellWidth?: number;
    //cellHeight?: number;
    onPointMove?: (index: number, oldX: number, oldY: number, newX: number, newY: number) => void;
    onPointClick?: (index: number, x: number, y: number, part: string) => void;
    onPointToggleVisibility?: (index: number, x: number, y: number, isNowVisible: boolean) => void;
}

export const PatternEditor: React.FC<PatternEditorProps> = ({
    draft,
    loopMap,
    width = 800,
    height = 600,
    //cellWidth = 20,
    //cellHeight = 25,
    onPointMove,
    onPointClick,
    onPointToggleVisibility,
}) => {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const [points, setPoints] = useState<Point[]>([]);
    const [activePointId, setActivePointId] = useState<string | null>(null);
    const dragStartPointRef = useRef<{ x: number; y: number; index: number } | null>(null);

    const [selectedPoints, setSelectedPoints] = useState<number[]>([]); // индексы выбранных опорных точек
    const [movingPointIndex, setMovingPointIndex] = useState<number | null>(null); // индекс перемещаемой точки
    const [selectionMode, setSelectionMode] = useState<'idle' | 'selectingAnchors' | 'selectingMoving'>('idle');
    const [phantomX, setPhantomX] = useState<number | null>(null); // временная X координата при перемещении
    const [phantomY, setPhantomY] = useState<number | null>(null); // временная Y координата при перемещении
    const [statusMessage, setStatusMessage] = useState<string>(''); // подсказка пользователю


    // Параметры масштабирования
    const [transformParams, setTransformParams] = useState<{
        scale: number;
        offsetX: number;
        offsetY: number;
    }>({ scale: 1, offsetX: 0, offsetY: 0 });

    // Если loopMap не передан или значения отсутствуют — используем значения по умолчанию
    const realCellWidth = loopMap?.loopWidth ?? 20;   // ширина петли в см (оригинальные координаты)
    const realCellHeight = loopMap?.loopHeight ?? 25; // высота петли в см (оригинальные координаты)

    // Синхронизация с пропсом draft
    useEffect(() => {
        if (draft?.draft) {
            // eslint-disable-next-line react-hooks/set-state-in-effect
            setPoints(draft.draft);
        }
    }, [draft]);

    // Вычисление параметров масштабирования на основе точек
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
        // eslint-disable-next-line react-hooks/set-state-in-effect
        setTransformParams(newParams);
    }, [points, width, height, calculateTransformParams]);



    // ==================== ОБРАБОТЧИК ДВОЙНОГО КЛИКА ====================

    // Находит точку под курсором (включая невидимые)
    const findAnyPointUnderCursor = useCallback((clientX: number, clientY: number): number | null => {
        const canvas = canvasRef.current;
        if (!canvas) return null;

        const rect = canvas.getBoundingClientRect();
        const mouseX = clientX - rect.left;
        const mouseY = clientY - rect.top;
        const { scale, offsetX, offsetY } = transformParams;

        // Ищем среди ВСЕХ точек (включая невидимые)
        for (let i = 0; i < points.length; i++) {
            const point = points[i];

            // Преобразуем координаты точки в canvas
            const canvasX = point.x * scale + offsetX;
            const canvasY = canvas.height - (point.y * scale + offsetY);

            // Проверяем расстояние от курсора до точки
            const dx = canvasX - mouseX;
            const dy = canvasY - mouseY;
            const dist = Math.sqrt(dx * dx + dy * dy);

            if (dist < 10) return i; // радиус захвата 10 пикселей
        }
        return null;
    }, [points, transformParams]);

    // Обработчик двойного клика
    const handleCanvasDoubleClick = (e: React.MouseEvent<HTMLCanvasElement>) => {
        const pointIndex = findAnyPointUnderCursor(e.clientX, e.clientY);

        if (pointIndex !== null) {
            const point = points[pointIndex];

            // 🟢 Переключаем видимость точки
            const newPoints = [...points];
            newPoints[pointIndex] = {
                ...point,
                visible: !point.visible  // инвертируем видимость
            };
            setPoints(newPoints);

            // Уведомляем родителя о изменении видимости
            const action = point.visible ? 'скрыта' : 'показана';
            console.log(`Точка ${pointIndex} ${action}: (${point.x}, ${point.y})`);

            if (onPointToggleVisibility) {
                onPointToggleVisibility(pointIndex, point.x, point.y, !point.visible);
            }
        }
    };




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

        // Сетка масштабируется вместе с чертежом
        const cellWidthPx = realCellWidth * scale;
        const cellHeightPx = realCellHeight * scale;

        // Рисуем сетку
        ctx.strokeStyle = '#e0e0e0';
        ctx.lineWidth = 1;

        // Находим первую точку для привязки сетки
        const firstPoint = points[0];
        if (firstPoint) {
            // Вычисляем позицию первой точки на canvas
            const firstX = firstPoint.x * scale + offsetX;
            const firstY = canvas.height - (firstPoint.y * scale + offsetY);

            // Рисуем вертикальные линии, проходящие через первую точку
            let startX = firstX % cellWidthPx;
            if (startX < 0) startX += cellWidthPx;
            for (let x = startX; x <= width; x += cellWidthPx) {
                ctx.beginPath();
                ctx.moveTo(x, 0);
                ctx.lineTo(x, height);
                ctx.stroke();
            }

            // Рисуем горизонтальные линии, проходящие через первую точку
            let startY = firstY % cellHeightPx;
            if (startY < 0) startY += cellHeightPx;
            for (let y = startY; y <= height; y += cellHeightPx) {
                ctx.beginPath();
                ctx.moveTo(0, y);
                ctx.lineTo(width, y);
                ctx.stroke();
            }
        } else {
            // Если нет точек — стандартная сетка от 0
            for (let x = 0; x <= width; x += cellWidthPx) {
                ctx.beginPath();
                ctx.moveTo(x, 0);
                ctx.lineTo(x, height);
                ctx.stroke();
            }
            for (let y = 0; y <= height; y += cellHeightPx) {
                ctx.beginPath();
                ctx.moveTo(0, y);
                ctx.lineTo(width, y);
                ctx.stroke();
            }
        }


        // ==================== ФАНТОМНЫЕ ЛИНИИ ====================
        // Рисуем фиолетовые пунктирные линии между опорными и перемещаемой точкой
        if (selectedPoints.length === 2 && movingPointIndex !== null && phantomX !== null && phantomY !== null) {
            const [leftIndex, rightIndex] = selectedPoints;
            const leftPoint = points[leftIndex];
            const rightPoint = points[rightIndex];

            if (leftPoint && rightPoint) {
                // Преобразуем координаты
                const leftX = leftPoint.x * scale + offsetX;
                const leftY = canvas.height - (leftPoint.y * scale + offsetY);
                const rightX = rightPoint.x * scale + offsetX;
                const rightY = canvas.height - (rightPoint.y * scale + offsetY);

                // Фантомная позиция перемещаемой точки
                const phantomCanvasX = phantomX * scale + offsetX;
                const phantomCanvasY = canvas.height - (phantomY * scale + offsetY);

                // Рисуем пунктирную линию от левой опорной до фантомной
                ctx.save();
                ctx.strokeStyle = '#a855f7'; // фиолетовый
                ctx.lineWidth = 2;
                ctx.setLineDash([5, 5]); // пунктир

                ctx.beginPath();
                ctx.moveTo(leftX, leftY);
                ctx.lineTo(phantomCanvasX, phantomCanvasY);
                ctx.stroke();

                // Рисуем пунктирную линию от фантомной до правой опорной
                ctx.beginPath();
                ctx.moveTo(phantomCanvasX, phantomCanvasY);
                ctx.lineTo(rightX, rightY);
                ctx.stroke();

                ctx.restore(); // восстанавливаем стиль (убираем пунктир)
            }
        }

        // ==================== ОСНОВНЫЕ ЛИНИИ ====================
        // Рисуем линии между ВСЕМИ точками (без фантомных в основной массив)
        if (points.length > 0) {
            ctx.beginPath();
            ctx.strokeStyle = '#000000';
            ctx.lineWidth = 2;

            for (let i = 0; i < points.length - 1; i++) {
                // Пропускаем отрезки, которые задействованы в фантомном перемещении
                const isPhantomSegment = selectedPoints.length === 2 && movingPointIndex !== null &&
                    ((i === selectedPoints[0] && i + 1 === movingPointIndex) ||
                        (i === movingPointIndex && i + 1 === selectedPoints[1]));

                if (isPhantomSegment && phantomX !== null) {
                    // Временно "разрываем" линию для фантомного отрезка
                    // Линия будет нарисована пунктиром отдельно
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

        // Точки рисуются через компонент DraggablePoint
    }, [points, width, height, draft, transformParams, realCellWidth, realCellHeight, selectedPoints, movingPointIndex, phantomX, phantomY]);

    // ==================== ОБРАБОТЧИК ВЫБОРА ТОЧЕК ====================

    // Проверка, находится ли точка между двумя опорными
    const isPointBetweenAnchors = useCallback((pointIndex: number, anchorIndices: number[]): boolean => {
        if (anchorIndices.length !== 2) return false;

        const [idx1, idx2] = anchorIndices;

        // Определяем левую и правую границу в списке
        const startIdx = Math.min(idx1, idx2);
        const endIdx = Math.max(idx1, idx2);

        // Точка находится между, если её индекс строго между границами
        return pointIndex > startIdx && pointIndex < endIdx;
    }, []);

    // Обработчик клика по точке (выбор опорных и перемещаемой)
    const handlePointSelection = (index: number, x: number, y: number, part: string) => {


        // Если есть перемещаемая точка — сбрасываем всё (начать заново)
        if (movingPointIndex !== null) {
            setSelectedPoints([]);
            setMovingPointIndex(null);
            setPhantomX(null);
            setPhantomY(null);
            setSelectionMode('idle');
            setStatusMessage('');
            return;
        }

        // Этап 1: выбор опорных точек
        if (selectedPoints.length < 2) {
            // Нельзя выбрать одну и ту же точку дважды
            if (selectedPoints.includes(index)) {
                // Снимаем выделение
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
                setStatusMessage('Выберите точку для перемещения ');
                setSelectionMode('selectingMoving');
            }
            return;
        }

        // Этап 2: выбор перемещаемой точки
        if (selectedPoints.length === 2) {
            if (isPointBetweenAnchors(index, selectedPoints)) {
                setMovingPointIndex(index);
                setSelectionMode('idle');
                setStatusMessage('Перемещайте точку');
            } else {
                setStatusMessage('Ошибка: выберите точку между двумя опорными!');
                setTimeout(() => {
                    setStatusMessage('Выберите точку для перемещения ');
                }, 2000);
            }
            return;
        }
    };

    // ==================== ОБРАБОТЧИКИ ПЕРЕТАСКИВАНИЯ ====================

    // Начало перетаскивания — если есть выбранные опорные и перемещаемая точка
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

        // Разрешаем перетаскивание ТОЛЬКО если это перемещаемая точка
        if (movingPointIndex === index && selectedPoints.length === 2) {
            setActivePointId(activeId);
        } else {
            // Запрещаем перетаскивание других точек
            if (movingPointIndex !== null) {
                setStatusMessage('Сначала завершите текущее перемещение (нажмите Escape)');
            } else if (selectionMode !== 'idle') {
                setStatusMessage('Сначала завершите выбор (нажмите Escape)');
            }
            return; // не разрешаем перетаскивание
        }
    }, [movingPointIndex, selectedPoints, points, selectionMode]);

    // Перемещение мыши (фантомное обновление позиции)
    const handleDragMove = useCallback((event: DragMoveEvent) => {

        // Если не в режиме перемещения — ничего не делаем
        if (movingPointIndex === null || selectedPoints.length !== 2) return;
        const { delta, active } = event;

        setActivePointId(String(active.id));

        if (movingPointIndex === null || selectedPoints.length !== 2) return;
        if (!dragStartPointRef.current) return;  // ДОБАВЛЕНО: проверка

        const startX = dragStartPointRef.current.x;
        const startY = dragStartPointRef.current.y;

        // Вычисляем новую позицию
        const deltaX = delta.x / transformParams.scale;
        const deltaY = -delta.y / transformParams.scale;

        let newX = startX + deltaX;
        let newY = startY + deltaY;

        // Привязка к сетке 
        const firstPoint = points[0];
        if (firstPoint) {
            const stepX = realCellWidth;
            const stepY = realCellHeight;
            newX = firstPoint.x + Math.round((newX - firstPoint.x) / stepX) * stepX;
            newY = firstPoint.y + Math.round((newY - firstPoint.y) / stepY) * stepY;
        }

        // Обновляем фантомные координаты (только для отображения пунктира)
        setPhantomX(newX);
        setPhantomY(newY);

    }, [movingPointIndex, selectedPoints, points, transformParams.scale, realCellWidth, realCellHeight]);

    // Окончание перетаскивания — отправляем запрос на бэкенд
    const handleDragEnd = useCallback(async (event: DragEndEvent) => {
        if (movingPointIndex === null || selectedPoints.length !== 2 || phantomX === null || phantomY === null) {
            setPhantomX(null);
            setPhantomY(null);
            dragStartPointRef.current = null;
            return;
        }

        // Оригинальная точка (до перемещения)
        const originalPoint = points[movingPointIndex];

        // Новые координаты (фантомные, которые получились при перетаскивании)
        const newX = phantomX;
        const newY = phantomY;
        const [leftIndex, rightIndex] = selectedPoints;
        const leftPoint = points[leftIndex];
        const rightPoint = points[rightIndex];

        if (!originalPoint || !leftPoint || !rightPoint) return;

        // Определяем, кто слева, кто справа по X
        const leftAnchor = leftPoint.x < rightPoint.x ? leftPoint : rightPoint;
        const rightAnchor = leftPoint.x < rightPoint.x ? rightPoint : leftPoint;

/*        // Создаём новую точку с обновлёнными координатами
        const movingPointData: Point = {
            x: phantomX,
            y: phantomY,
            visible: movingPoint.visible,
            part: movingPoint.part || ''
        };
*/
        try {
            setStatusMessage('Отправка изменений на сервер...');

            const newDraft = await DraftService.movePoint(
                originalPoint,  // старая точка (с исходными координатами)
                newX,           // новая X
                newY,           // новая Y
                leftAnchor,     // левая опорная точка
                rightAnchor     // правая опорная точка
            );


            // Обновляем чертёж
            if (newDraft && newDraft.draft) {
                setPoints(newDraft.draft);
                setStatusMessage('Точка перемещена!');
            }

            // Уведомляем родителя
            onPointMove?.(movingPointIndex, originalPoint.x, originalPoint.y, phantomX, phantomY);

        } catch (error) {
            console.error('Ошибка при перемещении точки:', error);
            setStatusMessage('Ошибка при перемещении точки');
        } finally {
            // Сбрасываем всё состояние
            setSelectedPoints([]);
            setMovingPointIndex(null);
            setPhantomX(null);
            setPhantomY(null);
            setActivePointId(null);
            dragStartPointRef.current = null;
            // Сбрасываем сообщение через 2 секунды
            setTimeout(() => setStatusMessage(''), 2000);
        }
    }, [movingPointIndex, selectedPoints, phantomX, phantomY, points, onPointMove]);

    // Отмена режима перемещения (через ESC или клик вне)
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

    // Обработчик клавиши ESC для отмены
    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') {
                cancelMovement();
            }
        };
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [cancelMovement]);

    // Комбинированный обработчик клика (отмена выбора)

    const handleCanvasClick = (e: React.MouseEvent<HTMLCanvasElement>) => {
        // Ищем точку под курсором
        const pointIndex = findPointUnderCursor(e.clientX, e.clientY);

        if (pointIndex !== null) {
            // Клик по точке — всегда выбор
            handlePointSelection(pointIndex, points[pointIndex].x, points[pointIndex].y, points[pointIndex].part || '');
        } else {
            // Клик мимо точек — отменяем текущий режим
            if (movingPointIndex !== null || selectedPoints.length > 0 || selectionMode !== 'idle') {
                cancelMovement();
            }
        }
    };

    // Поиск точки под курсором
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

    // Рендер подсказок (над canvas)
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
                    color: statusMessage.includes('Ошибка') ? '#ef4444' : '#666',
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
            <canvas
                ref={canvasRef}
                width={width}
                height={height}
                style={{ position: 'absolute', top: 0, left: 0, borderRadius: '8px' }}
                onDoubleClick={handleCanvasDoubleClick}  
                onClick={handleCanvasClick}
            />
            <DndContext onDragMove={handleDragMove} onDragEnd={handleDragEnd} onDragStart={handleDragStart} >
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
};