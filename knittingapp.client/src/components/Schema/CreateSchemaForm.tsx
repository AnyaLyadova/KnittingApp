import { useState } from 'react';


interface CreateSchemaFormProps {
    isOpen: boolean;
    onClose: () => void;
    onCreate: (name: string, m: number, n: number) => Promise<void>;
}

interface FormErrors {
    name?: string;
    width?: string;
    height?: string;
    submit?: string;
}

const CreateSchemaForm: React.FC<CreateSchemaFormProps> = ({ isOpen, onClose, onCreate }) => {
    const [name, setName] = useState('');
    const [width, setWidth] = useState(10);
    const [height, setHeight] = useState(10);
    const [errors, setErrors] = useState<FormErrors>({});

    if (!isOpen) return null;

    const validateForm = (): boolean => {
        const newErrors: FormErrors = {};

        if (!name.trim()) {
            newErrors.name = 'Введите название схемы';
        } else if (name.length > 50) {
            newErrors.name = 'Название не должно превышать 50 символов';
        }

        if (width < 1) {
            newErrors.width = 'Ширина должна быть больше 0';
        } else if (width > 50) {
            newErrors.width = 'Ширина не должна превышать 50';
        }

        if (height < 1) {
            newErrors.height = 'Высота должна быть больше 0';
        } else if (height > 50) {
            newErrors.height = 'Высота не должна превышать 50';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateForm()) {
            return;
        }

        try {
            await onCreate(name, width, height);
            setName('');
            setWidth(10);
            setHeight(10);
            setErrors({});
            onClose();
        } catch (error) {
            console.error('Ошибка создания схемы:', error);
            setErrors({ ...errors, submit: 'Не удалось создать схему' });
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <button className="modal-close" onClick={onClose}>✕</button>
                <h2>Создать новую схему</h2>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Название схемы:</label>
                        <input
                            type="text"
                            value={name}
                            onChange={(e) => {
                                setName(e.target.value);
                                if (errors.name) setErrors({ ...errors, name: undefined });
                            }}
                            placeholder="Введите название"
                            className={errors.name ? 'error' : ''}
                        />
                        {errors.name && <span className="error-message">{errors.name}</span>}
                    </div>

                    <div className="form-group">
                        <label>Ширина (клеток):</label>
                        <input
                            type="number"
                            value={width}
                            onChange={(e) => {
                                setWidth(parseInt(e.target.value) || 1);
                                if (errors.width) setErrors({ ...errors, width: undefined });
                            }}
                            min="1"
                            max="50"
                            className={errors.width ? 'error' : ''}
                        />
                        {errors.width && <span className="error-message">{errors.width}</span>}
                    </div>

                    <div className="form-group">
                        <label>Высота (клеток):</label>
                        <input
                            type="number"
                            value={height}
                            onChange={(e) => {
                                setHeight(parseInt(e.target.value) || 1);
                                if (errors.height) setErrors({ ...errors, height: undefined });
                            }}
                            min="1"
                            max="50"
                            className={errors.height ? 'error' : ''}
                        />
                        {errors.height && <span className="error-message">{errors.height}</span>}
                    </div>

                    {errors.submit && <div className="submit-error">{errors.submit}</div>}

                    <button type="submit" className="submit-btn">
                        Создать
                    </button>
                </form>
            </div>
        </div>
    );
};


export default CreateSchemaForm;