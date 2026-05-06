/* eslint-disable @typescript-eslint/no-unused-vars */
import React,{ useCallback, useEffect, useRef, useState } from 'react';
import ModelCreationForm from '../components/Constructor/ModelCreationForm';
import '../styles/ConstructorView.css'
import type { Model, Measures } from '../types/Model'
import type { Draft } from '../types/Draft'
import ModelList from '../components/Constructor/ModelList'
import { ConstructorService } from '../services/ConstructorService';
import { DraftService } from '../services/DraftService'
import InitializeMeasuresForm from '../components/Constructor/InitializeMeasuresForm'
import DraftField from '../components/Constructor/DraftField';
import { PatternEditor } from '../components/Constructor/PatternCanvas';
import type { LoopMap, Schema } from '../types/Schema';
import { ModelService } from '../services/ModelService';
import { DraftEditor, type DraftEditorRef } from '../components/Constructor/DraftEditor'
import ColorCircle from '../components/Schema/ColorCircle';
import SchemasList from '../components/Constructor/SchemasList';
import { flushSync } from 'react-dom';

interface ColorCircleValue {
    color: string;
    isEraser: boolean;
}
function ConstructorView() {
    // Состояние: открыто ли модальное окно
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedModel, setSelectedModel] = useState<Model | null>(null);
    const [refresher, setRefresher] = React.useState(0);

    // Состояния для модального окна редактирования мерок
    const [isMeasuresModalOpen, setIsMeasuresModalOpen] = useState(false);
    const [currentMeasures, setCurrentMeasures] = useState<Measures>({});


    // Состояние для цветового круга
    const [colorCircleValue, setColorCircleValue] = useState<ColorCircleValue>({
        color: '#007aff',
        isEraser: false
    });


    // состояния для схемы
    const [selectedSchema, setSelectedSchema] = useState<Schema | null>(null);
    const [isDraggingSchema, setIsDraggingSchema] = useState(false);
    const [dragPosition, setDragPosition] = useState({ x: 0, y: 0 });
    const [targetCell, setTargetCell] = useState<{ m: number; n: number } | null>(null);
    const [dragImageUrl, setDragImageUrl] = useState<string | null>(null);
    const [scaledImageUrl, setScaledImageUrl] = useState<string | null>(null); 


    const [gridCellWidth, setGridCellWidth] = useState(20);
    const [gridCellHeight, setGridCellHeight] = useState(25);

    const draftEditorRef = useRef<DraftEditorRef>(null);

    const [draftType, setDraftType] = useState("front");  //для текущего типа чертежа

    const [skipNextCellClick, setSkipNextCellClick] = useState(false);

    const [selectedSchemaId, setSelectedSchemaId] = useState<string | null>(null);




    // Обработчик выбора схемы из списка
  /*  const handleSelectSchema = (schemaId: string) => {
        setSelectedSchemaId(schemaId);
        // Можешь найти название схемы если нужно
        // const schema = schemas.find(s => s.schemaId === schemaId);
        // setSelectedSchemaName(schema?.schemaName || null);
    };*/

    //выбор модели
    const handleModelClick = async(model: Model) => {
        const id = model.modelId;
        console.log(`Клик по модели: ${model.name}, индекс: ${id}`);

        try {
            // Запрашиваем модель с сервера по индексу
            const fullModel = await ConstructorService.chooseModel(id);
            const measures = await ConstructorService.getMeasures();
            setSelectedModel(model);
            setCurrentMeasures(measures);
            setIsMeasuresModalOpen(true);
        }
        catch (err) {
            console.error('Ошибка загрузки мерок:', err);
        }
    };


    // Обработчик успешного сохранения мерок
    const handleMeasuresSaved = () => {
        setIsMeasuresModalOpen(false);
        setSelectedModel(null);
        setCurrentMeasures({});
        // Можно также обновить refresher, если нужно обновить что-то в списке
        // setRefresher(prev => prev + 1);
    };

    // Закрытие окна мерок
    const handleCloseMeasuresModal = () => {
        setIsMeasuresModalOpen(false);
        setSelectedModel(null);
        setCurrentMeasures({});
        loadDraft();
    };


    // Функция для получения текущего цвета/режима (передаётся в DraftEditor)
    const getCurrentColorMode = useCallback(() => {
        return {
            color: colorCircleValue.color,
            isEraser: colorCircleValue.isEraser
        };
    }, [colorCircleValue]);


    //переменные для чертежа
    const [draft, setDraft] = useState<Draft | null>(null);
    //переменная для матрицы петель
    const [loopMap, setLoopMap] = useState<LoopMap | null>(null);

  /*  const loadDraft = async () => {
        if (selectedModel === null) {
            setDraft(null);
            setLoopMap(null);
            return;
        }
        try {
            const data = await DraftService.createDrafts();
            setDraft(data);
            const map = await ModelService.getLoopMap(draftType);
            setLoopMap(map);
        } catch (err) {
            setDraft(null);
            setLoopMap(null);
        }
    }
*/

    //смена типа чертежа
    const switchDraft = useCallback(async (type: string) => {

        setSkipNextCellClick(true);

        try {
            let draftData: Draft | null = null;

            switch (type) {
                case "front":
                    draftData = await DraftService.getFrontDraft();
                    break;
                case "back":
                    draftData = await DraftService.getBackDraft();
                    break;
                case "sleeve":
                    draftData = await DraftService.getSleeveDraft();
                    break;
            }

            if (draftData) {
                setDraft(draftData);
                setDraftType(type);  // обновляем тип после успешной загрузки

                const map = await ModelService.getLoopMap(type);
                setLoopMap(map);
            }

        } catch (error) {
            console.error(`Ошибка загрузки чертежа ${type}:`, error);
        } finally {
            setTimeout(() => setSkipNextCellClick(false), 100);
        }
    }, []);

    const loadDraft = useCallback(async () => {
        if (selectedModel === null) {
            setDraft(null);
            setLoopMap(null);
            return;
        }
        try {
            await DraftService.createDrafts();
            await switchDraft("front");
        } catch (err) {
            console.error('Ошибка при создании чертежей:', err);
            setDraft(null);
            setLoopMap(null);
        }
    }, [selectedModel, switchDraft]);


    // Обработчики (могут быть пустыми, но должны быть переданы)
    const handlePointMove = (index: number, oldX: number, oldY: number, newX: number, newY: number) => {

    };

    const handlePointClick = (index: number, x: number, y: number, part: string) => {
    };

    const handlePointToggleVisibility = (index: number, x: number, y: number, isNowVisible: boolean) => {
    };

    const handleColorSchemeSaved = (newLoopMap: LoopMap) => {
        console.log('Цветовая схема сохранена');
        setLoopMap(newLoopMap);
    };


    // Загрузка изображения схемы
    const loadSchemaImage = useCallback((schema: Schema): Promise<HTMLImageElement> => {
        return new Promise((resolve, reject) => {
            const img = new Image();
            img.onload = () => resolve(img);
            img.onerror = reject;

            const imageSrc = schema.schemaImage?.startsWith('data:image')
                ? schema.schemaImage
                : `data:image/png;base64,${schema.schemaImage}`;
            img.src = imageSrc!;
        });
    }, []);



    // Масштабирование изображения под размеры клеток чертежа
    const scaleImageToGrid = useCallback(async (
        img: HTMLImageElement,
        schemaM: number,      // количество строк в схеме
        schemaN: number,      // количество столбцов в схеме
        targetCellWidthPx: number,   // целевая ширина клетки
        targetCellHeightPx: number   // целевая высота клетки
    ): Promise<string> => {
        // Вычисляем размер одной клетки в оригинальном изображении
        const originalCellWidth = img.width / schemaN;
        const originalCellHeight = img.height / schemaM;


        // Вычисляем коэффициенты масштабирования
        const scaleX = targetCellWidthPx / originalCellWidth;
        const scaleY = targetCellHeightPx / originalCellHeight;


        // Новые размеры изображения
        const newWidth = img.width * scaleX;
        const newHeight = img.height * scaleY;


        // Создаём canvas для масштабирования
        const canvas = document.createElement('canvas');
        canvas.width = newWidth;
        canvas.height = newHeight;
        const ctx = canvas.getContext('2d');

        if (!ctx) {
            throw new Error('Не удалось создать canvas контекст');
        }

        // Рисуем масштабированное изображение
        ctx.drawImage(img, 0, 0, newWidth, newHeight);

        // Возвращаем data URL
        return canvas.toDataURL('image/png');
    }, []);


    // Обработчик выбора схемы
    const handleSelectSchema = useCallback(async (schema: Schema) => {


        setSelectedSchema(schema);
        setDragImageUrl(null);
        setScaledImageUrl(null);

        try {
            // Загружаем изображение
            const imageSrc = schema.schemaImage?.startsWith('data:image')
                ? schema.schemaImage
                : `data:image/png;base64,${schema.schemaImage}`;

            // Создаём объект Image для получения размеров
            const img = new Image();

            await new Promise((resolve, reject) => {
                img.onload = resolve;
                img.onerror = reject;
                img.src = imageSrc;
            });

            // Масштабируем изображение под размеры клеток чертежа
            const scaledUrl = await scaleImageToGrid(
                img,
                schema.loopMap.m,        // количество строк в схеме
                schema.loopMap.n,        // количество столбцов в схеме
                gridCellWidth,           // 🟢 используем актуальное значение
                gridCellHeight           // 🟢 используем актуальное значение
            );

            setScaledImageUrl(scaledUrl);
            setDragImageUrl(scaledUrl);
            setIsDraggingSchema(true);
            setTargetCell(null);


        } catch (error) {
            console.error('Ошибка загрузки изображения схемы:', error);
        }
    }, [gridCellWidth, gridCellHeight, scaleImageToGrid]);  






    // Отмена перетаскивания
    const cancelDragging = useCallback(() => {
        setIsDraggingSchema(false);
        setSelectedSchema(null);
        setDragImageUrl(null);
        setScaledImageUrl(null);
        setTargetCell(null);
    }, []);

    // Обработчик клавиши ESC
    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape' && isDraggingSchema) {
                cancelDragging();
            }
        };
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [isDraggingSchema, cancelDragging]);




    // Обработчик начала перетаскивания (на mousedown на схеме)
    const handleSchemaMouseDown = useCallback((e: React.MouseEvent, schema: Schema) => {
        e.preventDefault();
        setIsDraggingSchema(true);
        setDragPosition({ x: e.clientX, y: e.clientY });
        setTargetCell(null);

        // Убедимся, что URL изображения установлен
        const imageSrc = schema.schemaImage?.startsWith('data:image')
            ? schema.schemaImage
            : `data:image/png;base64,${schema.schemaImage}`;
        setDragImageUrl(imageSrc);
    }, []);

    // Обработчик движения мыши (обновление позиции призрака)
    const handleMouseMove = useCallback((e: MouseEvent) => {
        if (!isDraggingSchema) return;
        setDragPosition({ x: e.clientX, y: e.clientY });
    }, [isDraggingSchema]);


    // Применение схемы к матрице чертежа
    const applySchemaToGrid = useCallback(() => {
        if (!selectedSchema || !targetCell) {
            console.log('Нет схемы или целевой ячейки');
            return;
        }

        const schemaMatrix = selectedSchema.loopMap?.loopMap;
        const schemaM = selectedSchema.loopMap?.m ?? 0;
        const schemaN = selectedSchema.loopMap?.n ?? 0;

        if (!schemaMatrix || schemaM === 0 || schemaN === 0) {
            console.log('Нет данных матрицы схемы');
            return;
        }

        console.log(`Применяем схему ${selectedSchema.schemaName}`, {
            startM: targetCell.m,
            startN: targetCell.n,
            schemaSize: { m: schemaM, n: schemaN }
        });

        // Проходим по всем ячейкам схемы
        for (let i = 0; i < schemaM; i++) {
            for (let j = 0; j < schemaN; j++) {
                const schemaCell = schemaMatrix[i]?.[j];
                const cellColor = schemaCell?.color;

                // Если цвет есть и он не none и не серый по умолчанию
                if (cellColor && cellColor !== 'none' && cellColor !== '#c0c0c0') {
                    const targetM = targetCell.m + i;
                    const targetN = targetCell.n + j;

                    if (draftEditorRef.current) {
                        flushSync(() => {
                            draftEditorRef.current?.colorCell(targetM, targetN, cellColor);
                        });
                    }
                    // TODO: здесь будет вызов функции окрашивания ячейки
                    // colorCell(targetM, targetN, cellColor);
                    console.log(`Должна окраситься ячейка (${targetM}, ${targetN}) в цвет ${cellColor}`);
                }
            }
        }

        console.log('Применение схемы завершено');
    }, [selectedSchema, targetCell]);


    // Обработчик отпускания мыши
    const handleMouseUp = useCallback(async (e: MouseEvent) => {
        if (!isDraggingSchema || !selectedSchema || !targetCell) {
            setIsDraggingSchema(false);
            return;
        }

        applySchemaToGrid();

        setTimeout(() => {
            setSelectedSchema(null);
            setDragImageUrl(null);
            setScaledImageUrl(null);
            setIsDraggingSchema(false);
            setTargetCell(null);
        }, 50);

        // Сбрасываем флаг через небольшой таймаут
        setTimeout(() => {
            setSkipNextCellClick(false);
        }, 100);
    }, [isDraggingSchema, selectedSchema, targetCell, applySchemaToGrid]);

    // Подписка на глобальные события мыши
    useEffect(() => {
        if (isDraggingSchema) {
            window.addEventListener('mousemove', handleMouseMove);
            window.addEventListener('mouseup', handleMouseUp);
            return () => {
                window.removeEventListener('mousemove', handleMouseMove);
                window.removeEventListener('mouseup', handleMouseUp);
            };
        }
    }, [isDraggingSchema, handleMouseMove, handleMouseUp]);

    // Функция для получения индекса ячейки под курсором (передаётся в DraftEditor)
    const handleCellHover = useCallback((mIndex: number, nIndex: number) => {
        if (isDraggingSchema) {
            setTargetCell({ m: mIndex, n: nIndex });
        }
    }, [isDraggingSchema]);

    const handleLoopMapUpdate = (newLoopMap: LoopMap) => {
        setLoopMap(newLoopMap);
    };


   

    // Загрузка чертежа при смене типа
   /* useEffect(() => {
        const loadDraftByType = async () => {
            if (selectedModel === null) return;

            try {
                let draftData: Draft | null = null;

                switch (draftType) {
                    case "front":
                        draftData = await DraftService.getFrontDraft();
                        break;
                    case "back":
                        draftData = await DraftService.getBackDraft();
                        break;
                    case "sleeve":
                        draftData = await DraftService.getSleeveDraft();
                        break;
                    default:
                        draftData = await DraftService.getFrontDraft();
                }

                setDraft(draftData);

                const map = await ModelService.getLoopMap(draftType);
                setLoopMap(map);

            } catch (error) {
                console.error(`Ошибка загрузки чертежа ${draftType}:`, error);
            }
        };

        loadDraftByType();
    }, [draftType, selectedModel]); // зависит от draftType и selectedModel*/


    return (
        <div className="constructor-view">
            <header className="constructor-header">
                <h1>Конструктор моделей</h1>
            </header>

            <main className="constructor-main">
                {/* Кнопка открытия формы */}
                <button
                    className="create-model-btn"
                    onClick={() => setIsModalOpen(true)}
                >
                    + Создать новую модель
                </button>

                {/*Список моделей*/ }
                <ModelList onModelClick={handleModelClick} />
            </main>

            {/* Модальное окно создания моделей*/}
            {isModalOpen && (
                <div className="modal-overlay" onClick={() => {
                    setIsModalOpen(false);
                    setRefresher(prev => prev + 1);
                }}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <button
                            className="modal-close"
                            onClick={() => {
                                setIsModalOpen(false);
                                setRefresher(prev => prev + 1);
                            }}
                        >
                            ✕
                        </button>
                        <ModelCreationForm onSuccess={() => {setIsModalOpen(false);
                            setRefresher(prev => prev + 1);
                            }} />
                    </div>
                </div>
            )}

            {/* Модальное окно задания мерок */}
            {isMeasuresModalOpen && selectedModel && (
                <div className="modal-overlay" onClick={handleCloseMeasuresModal}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <button
                            className="modal-close"
                            onClick={handleCloseMeasuresModal}
                        >
                            ✕
                        </button>
                        <InitializeMeasuresForm
                            modelIndex={selectedModel.modelId}
                            modelName={selectedModel.name}
                            initialMeasures={currentMeasures}
                            onSuccess={handleMeasuresSaved}
                            onClose={handleCloseMeasuresModal}

                        />
                    </div>
                </div>

            )}


            <div>
                <SchemasList
                    onSelectSchema={handleSelectSchema}
                />
            </div>

            {/* Панель с цветовым кругом */}
            <div className="right-panel">
                <div className="color-picker-section">
                    <ColorCircle
                        value={colorCircleValue}
                        onChange={setColorCircleValue}
                    />
                </div>
            </div>




            <div style={{ display: 'flex', gap: '10px', marginBottom: '10px', position: 'relative', zIndex: 10 }}>
                <button
                    onClick={() => switchDraft("front")}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: draftType === "front" ? '#007aff' : '#e0e0e0',
                        color: draftType === "front" ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Перед
                </button>
                <button
                    onClick={() => switchDraft("back")}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: draftType === "back" ? '#007aff' : '#e0e0e0',
                        color: draftType === "back" ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Спинка
                </button>
                <button
                    onClick={() => switchDraft("sleeve")}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: draftType === "sleeve" ? '#007aff' : '#e0e0e0',
                        color: draftType === "sleeve" ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Рукав
                </button>
            </div>

            <DraftEditor
                draft={draft}
                loopMap={loopMap}
                width={loopMap ? (loopMap.n * (loopMap.loopWidth || 20)*5 + 80) : 800}
                height={loopMap ? (loopMap.m * (loopMap.loopHeight || 25)*5 + 80) : 600}
                onPointMove={handlePointMove}
                onPointClick={handlePointClick}
                onPointToggleVisibility={handlePointToggleVisibility}
                currentColor={colorCircleValue.color}
                isEraserMode={colorCircleValue.isEraser}
                onColorSchemeSaved={handleColorSchemeSaved}
                onCellHover={handleCellHover} 

                onGridMetricsChange={(cellWidthPx, cellHeightPx) => {
                    setGridCellWidth(cellWidthPx);
                    setGridCellHeight(cellHeightPx);
                }}
                isDraggingSchema={isDraggingSchema}
                ref={draftEditorRef}
                onLoopMapUpdate={handleLoopMapUpdate}
                draftType={draftType}
            />

            {/* Призрак перетаскиваемой схемы */}
            {isDraggingSchema && scaledImageUrl && (
                <div
                    style={{
                        position: 'fixed',
                        left: dragPosition.x,
                        top: dragPosition.y,
                        pointerEvents: 'none',
                        zIndex: 1000,
                        opacity: 0.7,
                        transform: 'translate(0, 0)'
                    }}
                >
                    <img
                        src={scaledImageUrl}
                        alt="Dragging schema"
                        style={{
                            width: 'auto',
                            height: 'auto',
                            objectFit: 'contain'
                        }}
                    />
                </div>
            )}

        </div>

    );
}

export default ConstructorView;