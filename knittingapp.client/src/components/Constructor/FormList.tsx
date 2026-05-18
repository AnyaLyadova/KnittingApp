import { useState, useEffect } from 'react';
import { ConstructorService } from '../../services/ConstructorService';
import '../../styles/ModelList.css';
import type { Form } from '../../types/Form';

interface FormListProps {
    onModelClick?: (form: Form) => void;
}

export function FormList({ onModelClick }: FormListProps) {
    const [forms, setForms] = useState<Form[]>([]);

    useEffect(() => {
        const loadModels = async () => {
            try {
                const data = await ConstructorService.getModels();
                setForms(data);
            } catch (err) {
                console.error('Ошибка загрузки моделей:', err);
            }
        };
        loadModels();
    }, []);

    if (forms.length === 0) {
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
                {forms.map((form) => (
                    <button
                        key={form.formId}
                        className="model-item"
                        onClick={() => onModelClick?.(form)}
                    >
                        <div className="model-name">{form.name}</div>
                    </button>
                ))}
            </div>
        </div>
    );
}

export default FormList