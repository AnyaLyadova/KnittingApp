import React, { useState, useCallback } from 'react';
import ModelList from '../components/Account/ModelList';
import { DraftEditor} from '../components/Constructor/DraftEditor';
//import type { DraftEditorRef } from '../components/Constructor/DraftEditor';
import { DraftService } from '../services/DraftService';
import { ModelService } from '../services/ModelService';
import type { Draft } from '../types/Draft';
import type { LoopMap } from '../types/Schema';
import type { Model } from '../types/Model';
import ColorCircle from '../components/Schema/ColorCircle';
import '../styles/ConstructorView.css';
import ReaderField from '../components/Account/ReaderField';

interface ColorCircleValue {
    color: string;
    isEraser: boolean;
}

const AccountView: React.FC = () => {
    // Состояние для выбранной модели
    const [curModelId, setCurModelId] = useState<string | null>(null);
    const [currentModel, setCurrentModel] = useState<Model | null>(null);

    // Состояния для чертежа
    const [draft, setDraft] = useState<Draft | null>(null);
    const [loopMap, setLoopMap] = useState<LoopMap | null>(null);
    const [draftType, setDraftType] = useState<string>("front");

    // Состояние для цветового круга
    const [colorCircleValue, setColorCircleValue] = useState<ColorCircleValue>({
        color: '#007aff',
        isEraser: false
    });

    // Состояния для схем
    const [isDraggingSchema] = useState(false);
    const [gridCellWidth, setGridCellWidth] = useState(20);
    const [gridCellHeight, setGridCellHeight] = useState(25);

    // Обработчики
    const handlePointMove = () => { };
    const handlePointClick = () => { };
    const handlePointToggleVisibility = () => { };
    const handleColorSchemeSaved = (newLoopMap: LoopMap) => {
        setLoopMap(newLoopMap);
    };
    const handleCellHover = () => { };
    const handleLoopMapUpdate = (newLoopMap: LoopMap) => {
        setLoopMap(newLoopMap);
    };

    // Загрузка чертежа при выборе модели
    const loadDraftForModel = useCallback(async (modelId: string, type: string = "front") => {
        try {
            let draftData: Draft | null = null;

            switch (type) {
                case "front":
                    draftData = await DraftService.getFrontDraft(modelId);
                    break;
                case "back":
                    draftData = await DraftService.getBackDraft(modelId);
                    break;
                case "sleeve":
                    draftData = await DraftService.getSleeveDraft(modelId);
                    break;
            }

            if (draftData) {
                setDraft(draftData);
                setDraftType(type);

                const map = await ModelService.getLoopMap(modelId, type);
                setLoopMap(map);
            }
        } catch (err) {
            console.error('Ошибка загрузки чертежа:', err);
        }
    }, []);

    // Обработчик выбора модели
    const handleModelSelect = useCallback(async (modelId: string, model: Model) => {
        console.log(`Выбрана модель: ${modelId}`);
        setCurModelId(modelId);
        setCurrentModel(model);
        await loadDraftForModel(modelId, "front");
    }, [loadDraftForModel]);

    // Переключение типа чертежа
    const switchDraft = useCallback(async (type: string) => {
        if (!curModelId) return;
        await loadDraftForModel(curModelId, type);
    }, [curModelId, loadDraftForModel]);

    return (
        <div className="constructor-view">
            <header className="constructor-header">
                <h1>Мои модели</h1>
            </header>

            <main className="constructor-main">
                {/* Список моделей */}
                <ModelList onModelSelect={handleModelSelect} />
            </main>

            {/* Компонент чтения схемы */}
            {curModelId && (
                <ReaderField modelId={curModelId} initialType="front" />
            )}

            {/* Панель с цветовым кругом */}
            <div className="right-panel">
                <div className="color-picker-section">
                    <ColorCircle
                        value={colorCircleValue}
                        onChange={setColorCircleValue}
                    />
                </div>
            </div>


            {/* Кнопки переключения типа чертежа */}
            {curModelId && (
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
            )}

            {/* Компонент чертежа */}
            {curModelId && draft && loopMap && (
                <DraftEditor
                    draft={draft}
                    loopMap={loopMap}
                    width={loopMap ? (loopMap.n * (loopMap.loopWidth || 20) * 5 + 80) : 800}
                    height={loopMap ? (loopMap.m * (loopMap.loopHeight || 25) * 5 + 80) : 600}
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
                    onLoopMapUpdate={handleLoopMapUpdate}
                    draftType={draftType}
                    modelId={curModelId}
                />
            )}
        </div>
    );
};

export default AccountView;