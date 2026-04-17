import { useState, useEffect } from 'react';
import { ConstructorService } from '../../services/ConstructorService';
import type { Model } from '../../types/Model';
import '../../styles/ModelList.css';

interface ModelListProps {
    onModelClick?: (model: Model) => void;
}

export function ModelList({ onModelClick }: ModelListProps) {
    const [models, setModels] = useState<Model[]>([]);

    useEffect(() => {
        const loadModels = async () => {
            try {
                const data = await ConstructorService.getModels();
                setModels(data);
            } catch (err) {
                console.error('Ошибка загрузки моделей:', err);
            }
        };
        loadModels();
    }, []);

    if (models.length === 0) {
        return (
            <div className="models-list-container">
                <h3>Ваши модели</h3>
                <div className="empty-models">Нет созданных моделей</div>
            </div>
        );
    }

    return (
        <div className="models-list-container">
            <h3>Ваши модели</h3>
            <div className="models-scroll">
                {models.map((model) => (
                    <button
                        key={model.modelId}
                        className="model-item"
                        onClick={() => onModelClick?.(model)}
                    >
                        <div className="model-name">{model.name}</div>
                    </button>
                ))}
            </div>
        </div>
    );
}

export default ModelList