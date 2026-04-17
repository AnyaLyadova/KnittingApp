/* eslint-disable @typescript-eslint/no-unused-vars */
import React,{ useState } from 'react';
import ModelCreationForm from '../components/Constructor/ModelCreationForm';
import '../styles/ConstructorView.css'
import type { Model, Measures } from '../types/Model'
import type { Draft } from '../types/Draft'
import ModelList from '../components/Constructor/ModelList'
import { ConstructorService } from '../services/ConstructorService';
import { DraftService } from '../services/DraftService'
import InitializeMeasuresForm from '../components/Constructor/InitializeMeasuresForm'
import DraftField from '../components/Constructor/DraftField';

function ConstructorView() {
    // Состояние: открыто ли модальное окно
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedModel, setSelectedModel] = useState<Model | null>(null);
    const [refresher, setRefresher] = React.useState(0);

    // Состояния для модального окна редактирования мерок
    const [isMeasuresModalOpen, setIsMeasuresModalOpen] = useState(false);
    const [currentMeasures, setCurrentMeasures] = useState<Measures>({});

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


    //переменные для чертежа
    const [draft, setDraft] = useState<Draft | null>(null);

    const loadDraft = async () => {
        if (selectedModel === null) {
            setDraft(null);
            return;
        }
        try {
            const data = await DraftService.getDraft();
            setDraft(data);
        } catch (err) {
            setDraft(null);
        }
    }



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


            {/* Область с чертежом */}
            <div style={{ marginTop: '20px' }}>

                <DraftField draft={draft} width={800} height={600} />

            </div>

        </div>

    );
}

export default ConstructorView;