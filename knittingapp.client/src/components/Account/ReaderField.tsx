import React, { useState, useEffect, useCallback, useRef } from 'react';
import ReaderService from '../../services/ReaderService';

interface ReaderFieldProps {
    modelId: string;
    initialType?: string;
}

const ReaderField: React.FC<ReaderFieldProps> = ({ modelId, initialType = 'front' }) => {
    // Состояния
    const [currentType, setCurrentType] = useState<string>(initialType);
    const [readerId, setReaderId] = useState<string | null>(null);
    const [currentString, setCurrentString] = useState<string[]>([]);
    const [colors, setColors] = useState<string[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    // Состояния для таймера
    const [spentTime, setSpentTime] = useState<number>(0); // в секундах
    const [isTimerRunning, setIsTimerRunning] = useState<boolean>(false);
    const timerIntervalRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    // Загрузка readerId при монтировании или смене типа
    const loadReaderId = useCallback(async (type: string) => {
        if (!modelId) return;

        try {
            setIsLoading(true);
            const id = await ReaderService.getLoopReader(modelId, type);
            setReaderId(id);
            return id;
        } catch (err) {
            console.error('Ошибка загрузки readerId:', err);
            setError('Не удалось загрузить ридер');
            return null;
        } finally {
            setIsLoading(false);
        }
    }, [modelId]);

    // Загрузка текущей строки
    const loadCurrentString = useCallback(async (id: string) => {
        if (!id) return;

        try {
            const [stringSegments, stringColors] = await ReaderService.getCurrentString(id);
            setCurrentString(stringSegments);
            setColors(stringColors);
        } catch (err) {
            console.error('Ошибка загрузки текущей строки:', err);
            setError('Не удалось загрузить строку');
        }
    }, []);

    // Загрузка прогресса и времени при получении readerId
    const loadReaderData = useCallback(async (id: string) => {
        if (!id) return;

        try {
            // Загружаем время
            const timeStr = await ReaderService.getSpentTime(id);
            // Преобразуем строку времени в секунды (формат "hh:mm:ss")
            const parts = timeStr.split(':');
            const seconds = parseInt(parts[0]) * 3600 + parseInt(parts[1]) * 60 + parseInt(parts[2]);
            setSpentTime(seconds);

            // Загружаем первую строку
            await loadCurrentString(id);
        } catch (err) {
            console.error('Ошибка загрузки данных ридера:', err);
        }
    }, [loadCurrentString]);

    // Переключение типа чертежа
    const switchType = useCallback(async (type: string) => {
        setCurrentType(type);
        setCurrentString([]);
        setColors([]);

        const newReaderId = await loadReaderId(type);
        if (newReaderId) {
            await loadReaderData(newReaderId);
        }
    }, [loadReaderId, loadReaderData]);

    // Переход к следующему ряду
    const handleMoveNext = useCallback(async () => {
        if (!readerId) return;

        try {
            setIsLoading(true);
            await ReaderService.moveNext(readerId);
            await loadCurrentString(readerId);
        } catch (err) {
            console.error('Ошибка перехода к следующему ряду:', err);
            setError('Не удалось перейти к следующему ряду');
        } finally {
            setIsLoading(false);
        }
    }, [readerId, loadCurrentString]);

    // Таймер: запуск
    const startTimer = useCallback(() => {
        if (timerIntervalRef.current) return;

        timerIntervalRef.current = setInterval(() => {
            setSpentTime(prev => prev + 1);
        }, 1000);
        setIsTimerRunning(true);
    }, []);

    // Таймер: остановка
    const stopTimer = useCallback(async () => {
        if (timerIntervalRef.current) {
            clearInterval(timerIntervalRef.current);
            timerIntervalRef.current = null;
        }
        setIsTimerRunning(false);

        // Отправляем время на сервер
        if (readerId) {
            try {
                const timeString = new Date(spentTime * 1000).toISOString().substr(11, 8);
                await ReaderService.setSpentTime(readerId, timeString);
            } catch (err) {
                console.error('Ошибка сохранения времени:', err);
            }
        }
    }, [readerId, spentTime]);

    // Инициализация при монтировании
    useEffect(() => {
        const init = async () => {
            const newReaderId = await loadReaderId(currentType);
            if (newReaderId) {
                await loadReaderData(newReaderId);
            }
        };

        init();

        // Очистка при размонтировании
        return () => {
            if (timerIntervalRef.current) {
                clearInterval(timerIntervalRef.current);
            }
            // Отправляем время при закрытии
            if (readerId && spentTime > 0) {
                const timeString = new Date(spentTime * 1000).toISOString().substr(11, 8);
                ReaderService.setSpentTime(readerId, timeString).catch(console.error);
            }
        };
    }, []); // Пустой массив — только при монтировании

    // Форматирование времени
    const formatTime = (seconds: number): string => {
        const hours = Math.floor(seconds / 3600);
        const minutes = Math.floor((seconds % 3600) / 60);
        const secs = seconds % 60;
        return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    };

    return (
        <div className="reader-field" style={{ padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h3>Чтение схемы</h3>

            {/* Кнопки переключения типа */}
            <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
                <button
                    onClick={() => switchType('front')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: currentType === 'front' ? '#007aff' : '#e0e0e0',
                        color: currentType === 'front' ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Перед
                </button>
                <button
                    onClick={() => switchType('back')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: currentType === 'back' ? '#007aff' : '#e0e0e0',
                        color: currentType === 'back' ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Спинка
                </button>
                <button
                    onClick={() => switchType('sleeve')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: currentType === 'sleeve' ? '#007aff' : '#e0e0e0',
                        color: currentType === 'sleeve' ? 'white' : '#333',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: 'pointer'
                    }}
                >
                    Рукав
                </button>
            </div>

            {/* Таймер */}
            <div style={{ display: 'flex', gap: '10px', alignItems: 'center', marginBottom: '20px' }}>
                <span style={{ fontSize: '24px', fontFamily: 'monospace' }}>{formatTime(spentTime)}</span>
                {!isTimerRunning ? (
                    <button
                        onClick={startTimer}
                        style={{
                            padding: '6px 12px',
                            backgroundColor: '#28a745',
                            color: 'white',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer'
                        }}
                    >
                        Запустить
                    </button>
                ) : (
                    <button
                        onClick={stopTimer}
                        style={{
                            padding: '6px 12px',
                            backgroundColor: '#dc3545',
                            color: 'white',
                            border: 'none',
                            borderRadius: '6px',
                            cursor: 'pointer'
                        }}
                    >
                        Остановить
                    </button>
                )}
            </div>

            {/* Кнопка "Следующий ряд" */}
            <div style={{ marginBottom: '20px' }}>
                <button
                    onClick={handleMoveNext}
                    disabled={isLoading || !readerId}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: '#007aff',
                        color: 'white',
                        border: 'none',
                        borderRadius: '6px',
                        cursor: isLoading ? 'not-allowed' : 'pointer',
                        opacity: isLoading ? 0.6 : 1
                    }}
                >
                    {isLoading ? 'Загрузка...' : 'Следующий ряд'}
                </button>
            </div>

            {/* Отображение ошибки */}
            {error && (
                <div style={{ color: 'red', marginBottom: '10px' }}>
                    {error}
                </div>
            )}

            {/* Отображение текущей строки */}
            <div className="current-string" style={{ marginTop: '20px' }}>
                <h4>Текущий ряд:</h4>
                <div style={{
                    display: 'flex',
                    flexWrap: 'wrap',
                    gap: '8px',
                    alignItems: 'center',
                    padding: '10px',
                    backgroundColor: '#f9f9f9',
                    borderRadius: '8px',
                    minHeight: '60px'
                }}>
                    {currentString.map((segment, index) => (
                        <span
                            key={index}
                            style={{
                                color: colors[index] || '#000000',
                                fontSize: '16px',
                                fontWeight: '500'
                            }}
                        >
                            {segment}
                            {index < currentString.length - 1 && <span style={{ color: '#000', marginLeft: '8px' }}>, </span>}
                        </span>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default ReaderField;