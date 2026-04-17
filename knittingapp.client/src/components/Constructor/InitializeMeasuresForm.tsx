import { useState } from 'react';
import type { Measures } from '../../types/Model';
import {ConstructorService} from '../../services/ConstructorService';

interface InitializeMeasuresFormProps {
    modelIndex: string;
    modelName: string;
    initialMeasures: Measures;
    onSuccess?: () => void;
    onClose?: () => void;
}

export function InitializeMeasuresForm({ modelIndex, modelName, initialMeasures, onSuccess, onClose }: InitializeMeasuresFormProps) {

    //мерки для изделия
    const [measures, setMeasures] = useState<Measures>(initialMeasures);

    const [errors, setErrors] = useState<Record<string, string>>({});
    const [generalError, setGeneralError] = useState<string>('');

    //высота и ширина образца
    const [height, setHeight] = useState<number>(0);
    const [width, setWidth] = useState<number>(0);
    const [loopInHeight, setLoopInHeight] = useState<number>(0);
    const [loopInWidth, setLoopInWidth] = useState<number>(0);

    // Валидация одного поля
    const validateField = (name: string, value: number): string => {
        if (isNaN(value)) {
            return 'Введите число';
        }
        if (value === 0) {
            return 'Значение не может быть равно 0';
        }
        if (value < 0) {
            return 'Значение не может быть отрицательным';
        }
        return '';
    };

    // Валидация всех полей
    const validateAllFields = (): boolean => {
        const newErrors: Record<string, string> = {};
        let isValid = true;

        // Валидация мерок
        Object.entries(measures).forEach(([key, value]) => {
            const error = validateField(key, value);
            if (error) {
                newErrors[key] = error;
                isValid = false;
            }
        });

        // Валидация образца
        const heightError = validateField('height', height);
        if (heightError) {
            newErrors['height'] = heightError;
            isValid = false;
        }

        const widthError = validateField('width', width);
        if (widthError) {
            newErrors['width'] = widthError;
            isValid = false;
        }
        const loopHeightError = validateField('loopInHeight', loopInHeight);
        if (loopHeightError) {
            newErrors['loopInHeight'] = loopHeightError;
            isValid = false;
        }

        const loopWidthError = validateField('loopInWidth', loopInWidth);
        if (loopWidthError) {
            newErrors['loopInWidth'] = loopWidthError;
            isValid = false;
        }

        setErrors(newErrors);
        return isValid;
    };


    // Обработчик изменения значения мерки
    const handleMeasureChange = (key: string, value: string) => {
        const numValue = parseFloat(value);

        setMeasures(prev => ({
            ...prev,
            [key]: isNaN(numValue) ? 0 : numValue
        }));

        if (errors[key]) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors[key];
                return newErrors;
            });
        }
    };


    // Обработчики изменения для образца
    const handleHeightChange = (value: string) => {
        const numValue = parseFloat(value);
        setHeight(isNaN(numValue) ? 0 : numValue);
        if (errors['height']) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors['height'];
                return newErrors;
            });
        }
    };

    const handleWidthChange = (value: string) => {
        const numValue = parseFloat(value);
        setWidth(isNaN(numValue) ? 0 : numValue);
        if (errors['width']) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors['width'];
                return newErrors;
            });
        }
    };

    const handleLoopInHeightChange = (value: string) => {
        const numValue = parseInt(value, 10);
        setLoopInHeight(isNaN(numValue) ? 0 : numValue);
        if (errors['loopInHeight']) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors['loopInHeight'];
                return newErrors;
            });
        }
    };

    const handleLoopInWidthChange = (value: string) => {
        const numValue = parseInt(value, 10);
        setLoopInWidth(isNaN(numValue) ? 0 : numValue);
        if (errors['loopInWidth']) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors['loopInWidth'];
                return newErrors;
            });
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validateAllFields()) {
            return;
        }

        setGeneralError('');

        try {

            console.log('Отправка мерок для модели', modelIndex);
            ConstructorService.sendMeasures(measures, height, width, loopInHeight, loopInWidth);
            console.log('Мерки:', measures);


            if (onSuccess) {
                onSuccess();
            }
            if (onClose) {
                onClose();
            }
        } catch (err) {
            console.error('Ошибка:', err);
            setGeneralError('Ошибка при сохранении мерок');
        }
    };

    return (
        <form className="model-creation-form" onSubmit={handleSubmit}>
            <h2>Редактирование мерок</h2>
            <p>Модель: {modelName}</p>

            {/* Блок с параметрами образца */}
            <div className="sample-parameters">
                <h3>Параметры образца</h3>

                <div className="form-group">
                    <label>Высота образца (см):</label>
                    <input
                        type="number"
                        step="0.1"
                        value={height}
                        onChange={(e) => handleHeightChange(e.target.value)}
                        className={errors['height'] ? 'error-input' : ''}
                    />
                    {errors['height'] && <div className="field-error">{errors['height']}</div>}
                </div>

                <div className="form-group">
                    <label>Ширина образца (см):</label>
                    <input
                        type="number"
                        step="0.1"
                        value={width}
                        onChange={(e) => handleWidthChange(e.target.value)}
                        className={errors['width'] ? 'error-input' : ''}
                    />
                    {errors['width'] && <div className="field-error">{errors['width']}</div>}
                </div>

                <div className="form-group">
                    <label>Высота образца (петли):</label>
                    <input
                        type="number"
                        step="1"
                        value={loopInHeight}
                        onChange={(e) => handleLoopInHeightChange(e.target.value)}
                        className={errors['loopInHeight'] ? 'error-input' : ''}
                    />
                    {errors['loopInHeight'] && <div className="field-error">{errors['loopInHeight']}</div>}
                </div>

                <div className="form-group">
                    <label>Ширина образца (петли):</label>
                    <input
                        type="number"
                        step="1"
                        value={loopInWidth}
                        onChange={(e) => handleLoopInWidthChange(e.target.value)}
                        className={errors['loopInWidth'] ? 'error-input' : ''}
                    />
                    {errors['loopInWidth'] && <div className="field-error">{errors['loopInWidth']}</div>}
                </div>
            </div>

            {/* Блок с мерками */}
            <div className="measures-parameters">
                <h3>Мерки (см)</h3>
                <div className="measures-fields">
                {Object.entries(measures).map(([key, value]) => (
                    <div key={key} className="form-group">
                        <label htmlFor={key}>
                            {key}:
                        </label>
                        <input
                            id={key}
                            type="number"
                            step="0.1"
                            value={value}
                            onChange={(e) => handleMeasureChange(key, e.target.value)}
                            className={errors[key] ? 'error-input' : ''}
                        />
                        {errors[key] && <div className="field-error">{errors[key]}</div>}
                    </div>

                ))}
                </div>
            </div>

            {generalError && <div className="form-error server-error">{generalError}</div>}

            <div className="form-buttons">
                <button type="submit">
                    Сохранить
                </button>
            </div>
        </form>
    );
}
export default InitializeMeasuresForm;