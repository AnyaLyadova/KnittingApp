import React, { useState, useEffect } from 'react';
import { ModelService } from '../../services/ModelService';
import type { Model } from '../../types/Model';

interface ModelListProps {
    onModelSelect: (modelId: string, model: Model) => void;
}

const ModelList: React.FC<ModelListProps> = ({ onModelSelect }) => {
    const [models, setModels] = useState<Model[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedModelId, setSelectedModelId] = useState<string | null>(null);

    const loadModels = async () => {
        try {
            setLoading(true);
            const modelList = await ModelService.getModels();
            setModels(modelList);
            setError(null);
        } catch (err) {
            console.error('Ошибка загрузки моделей:', err);
            setError('Не удалось загрузить список моделей');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadModels();
        // eslint-disable-next-line react-hooks/set-state-in-effect
    }, []);


    const handleModelClick = (model: Model) => {
        setSelectedModelId(model.modelId);
        onModelSelect(model.modelId, model);
    };

    if (loading) {
        return (
            <div className="model-list">
                <div className="loading">Загрузка моделей...</div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="model-list">
                <div className="error">{error}</div>
                <button onClick={loadModels}>Повторить</button>
            </div>
        );
    }

    if (models.length === 0) {
        return (
            <div className="model-list">
                <div className="empty">У вас пока нет моделей</div>
            </div>
        );
    }

    return (
        <div className="model-list">
            <div className="models-grid">
                {models.map((model) => (
                    <button
                        key={model.modelId}
                        className={`model-btn ${selectedModelId === model.modelId ? 'active' : ''}`}
                        onClick={() => handleModelClick(model)}
                    >
                        {model.name}
                    </button>
                ))}
            </div>
        </div>
    );
};

export default ModelList;