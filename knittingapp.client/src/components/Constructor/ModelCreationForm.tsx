import { useState, useEffect } from 'react';
import { ModelService } from '../../services/ModelService';
import type { NeckParts, SleeveRollParts, ArmholeParts, Model } from '../../types/Model';
import '../../styles/Modal.css';


interface ModelCreationFormProps {
    onSuccess?: () => void;  
}
function ModelCreationForm({ onSuccess }: ModelCreationFormProps) {
         // Состояния для хранения данных с бэка
    const [neckParts, setNeckParts] = useState<NeckParts>({});
    const [sleeveRollParts, setSleeveRollParts] = useState<SleeveRollParts>({});
    const [armholeParts, setArmholeParts] = useState<ArmholeParts>({});

    const [validationError, setValidationError] = useState<string>('');  // добавить
    const [serverError, setServerError] = useState<string>('');  

    // Состояния для загрузки и ошибок
    const [error, setError] = useState<string | null>(null);

    // Состояния для имени модели (пока просто храним)
    const [modelName, setModelName] = useState('');

    // Состояния для выбранных значений
    const [selectedNeck, setSelectedNeck] = useState<string>('');
   // const [selectedSleeveRoll, setSelectedSleeveRoll] = useState<string>('');
    const [selectedArmhole, setSelectedArmhole] = useState<string>('');

    

    // Загружаем данные при монтировании компонента
    useEffect(() => {
        const loadData = async () => {
            try {
                setError(null);

                const [neck, /*sleeveRoll,*/ armhole] = await Promise.all([
                    ModelService.getNeckParts(),
                 //   ModelService.getSleeveRollParts(),
                    ModelService.getArmholeParts()
                ]);

                setNeckParts(neck);
               // setSleeveRollParts(sleeveRoll);
                setArmholeParts(armhole);

                const neckKeys = Object.keys(neck);
               // const sleeveKeys = Object.keys(sleeveRoll);
                const armholeKeys = Object.keys(armhole);

                if (neckKeys.length > 0) setSelectedNeck(neckKeys[0]);
               // if (sleeveKeys.length > 0) setSelectedSleeveRoll(sleeveKeys[0]);
                if (armholeKeys.length > 0) setSelectedArmhole(armholeKeys[0]);

            } catch (err) {
                setError(err instanceof Error ? err.message : 'Ошибка загрузки данных');
                console.error('Ошибка загрузки:', err);
            } 
        };

        loadData();
    }, []); 

    // Функция для сборки массива частей
    const buildPartsArray = (): string[] => {
        const parts: string[] = [];

        // Добавляем выбранную горловину (если выбрана)
        if (selectedNeck) {
            parts.push(selectedNeck);
        }

        // Добавляем выбранный скос рукава (если выбран)
       /* if (selectedSleeveRoll) {
            parts.push(selectedSleeveRoll);
        }*/

        // Добавляем выбранную выемку (если выбрана)
        if (selectedArmhole) {
            parts.push(selectedArmhole);
        }

        return parts;
    };


    const validateForm = (): boolean => {  //валидация формы
        if (!modelName.trim()) {
            setValidationError('Введите название модели');
            return false;
        }
      /*  if (!selectedNeck || !selectedSleeveRoll || !selectedArmhole) {
            setValidationError('Выберите все параметры модели');
            return false;
        }*/
        setValidationError('');
        return true;
    };

    // Отправка формы
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

      /*  // Простая проверка: имя не должно быть пустым
        if (!modelName.trim()) {
            alert('Введите название модели');
            return;
        }*/

        setServerError('');   //очистка прошлых ошибок
        if (!validateForm()) return;  //проверка валидации формы

        try {
            const requestData: Model = {
                modelId:"000",
                name: modelName,
                parts: buildPartsArray()
            };

            const response = await ModelService.createModel(requestData);
            console.log('Модель успешно создана:', response);
            //  alert('Модель успешно создана!');
            // Очищаем ошибки перед закрытием
            setValidationError('');
            setServerError('');
            //Закрываем модальное окно при успешном создании
            if (onSuccess) {
                onSuccess();
            }

        } catch (err: unknown) {  // eslint-disable-line
            console.error('Ошибка:', err);
            /*const errorMessage = err.response?.data?.message || err.message || 'Ошибка при создании модели';
            alert(errorMessage);*/
            //   alert('Ошибка при создании модели');

            let errorMessage = 'Ошибка при создании модели';
            if (err && typeof err === 'object' && 'response' in err) {
                const axiosError = err as { response?: { data?: string } };
                if (axiosError.response?.data) {
                    errorMessage = axiosError.response.data;
                }
            } else if (err instanceof Error) {
                errorMessage = err.message;
            }
            /*if (err && typeof err === 'object' && 'response' in err) {
                const axiosError = err as { response?: { data?: { message?: string } } };
                if (axiosError.response?.data?.message) {
                    errorMessage = axiosError.response.data.message;
                }
            } else if (err instanceof Error) {
                errorMessage = err.message;
            }*/


            setServerError(errorMessage);
        }
    };


    // Показываем ошибку
    if (error) {
        return (
            <div className="error-container">
                <p>Ошибка: {error}</p>
                <button onClick={() => window.location.reload()}>Попробовать снова</button>
            </div>
        );
    }

    return (
        <form className="create-model-form">
            <h2>Создание новой модели</h2>

            {/* Поле ввода названия модели */}
            <div className="form-group">
                <label htmlFor="modelName">Название модели:</label>
                <input
                    id="modelName"
                    type="text"
                    value={modelName}
                    onChange={(e) => {
                        setModelName(e.target.value);
                        if (validationError) setValidationError('');
                    }}
                    placeholder="Введите название модели"
                />
            </div>

            {/* Блок 1: Части шеи / Горловина */}
            <div className="form-group">
                <label className="block-label">Горловина:</label>
                <div className="radio-group">
                    {Object.entries(neckParts).map(([key, value]) => (
                        <label key={key} className="radio-option">
                            <input
                                type="radio"
                                name="neckPart"
                                value={key}
                                checked={selectedNeck === key}
                                onChange={(e) => {setSelectedNeck(e.target.value);
                        if (validationError) setValidationError('');}}
                            />
                            <span>{value}</span>
                        </label>
                    ))}
                </div>
            </div>

            {/* Блок 2: Скос рукава */}
           {/* <div className="form-group">
                <label className="block-label">Скос рукава:</label>
                <div className="radio-group">
                    {Object.entries(sleeveRollParts).map(([key, value]) => (
                        <label key={key} className="radio-option">
                            <input
                                type="radio"
                                name="sleeveRoll"
                                value={key}
                                checked={selectedSleeveRoll === key}
                                onChange={(e) => {setSelectedSleeveRoll(e.target.value);
                        if (validationError) setValidationError('');}}
                            />
                            <span>{value}</span>
                        </label>
                    ))}
                </div>
            </div>*/}

            {/* Блок 3: Выемка под рукав */}
            <div className="form-group">
                <label className="block-label">Выемка под рукав:</label>
                <div className="radio-group">
                    {Object.entries(armholeParts).map(([key, value]) => (
                        <label key={key} className="radio-option">
                            <input
                                type="radio"
                                name="armhole"
                                value={key}
                                checked={selectedArmhole === key}
                                onChange={(e) => {setSelectedArmhole(e.target.value);
                        if (validationError) setValidationError('');}}
                            />
                            <span>{value}</span>
                        </label>
                    ))}
                </div>
            </div>

            {validationError && <div className="form-error">{validationError}</div>}
            {serverError && <div className="form-error server-error">{serverError}</div>}

            <button
                type="submit"
                onClick={handleSubmit}>
                Создать модель
            </button>
        </form>
  );
}

export default ModelCreationForm;