import { useState, useEffect, useRef } from 'react';
import { LoopMap } from '../../types/Schema';
import { SchemaService } from '../../services/SchemaService';
//import './SchemaField.css';

interface SchemaFieldProps {
    schemaId: string | null;
    selectedColor: string;
    schemaName?: string | null;
    isEraser: boolean;
    onPixelChange?: () => void;
}
interface LoopData {
    m: number;
    n: number;
    color: string;
    type: string;
}


const SchemaField: React.FC<SchemaFieldProps> = ({
    schemaId,
    selectedColor,
    isEraser,
    schemaName,
    onPixelChange
}) => {
    const [loopMap, setLoopMap] = useState<LoopMap | null>(null);
    const [isDrawing, setIsDrawing] = useState(false);
    const lastSentRef = useRef<{ x: number; y: number; color: string } | null>(null);

    // Загрузка схемы при изменении schemaId
    useEffect(() => {
        const loadSchema = async () => {
            if (!schemaId) {
                setLoopMap(null);
                return;
            }

            try {
                const schema = await SchemaService.getSchemaById(schemaId);
                console.log(schema)
                const loopMatrix = schema.loopMap.loopMap as unknown as LoopData[][]; //массив массивов

                const m = loopMatrix.length;       // количество строк
                const n = loopMatrix[0]?.length || 0; // количество столбцов
                const newLoopMap = new LoopMap(m, n);
              //  const newLoopMap = new LoopMap(schema.loopMap.m, schema.loopMap.n);

                

                // Копируем данные из полученной схемы
                for (let i = 0; i < m; i++) {
                    for (let j = 0; j < n; j++) {
                        const loop = loopMatrix[i]?.[j];
                        if (loop) {
                            // Если цвет "none" - заменяем на белый
                            let savedColor = loop.color || loop.color || '#FFFFFF';
                            if (savedColor === 'none') {
                                savedColor = '#FFFFFF';
                            }
                            const savedType = loop.type || 'loop';
                            newLoopMap.changeLoopColor(i, j, savedColor);
                            newLoopMap.changeLoopType(i, j, savedType);
                        }
                    }
                }
                setLoopMap(newLoopMap);
            } catch (error) {
                console.error('Ошибка загрузки схемы:', error);
                setLoopMap(null);
            }
        };

        loadSchema();
    }, [schemaId]);

    // Обработчик изменения цвета пикселя
    const handlePixelChange = async (x: number, y: number) => {
        if (!loopMap || !schemaId) return;

        const currentLoop = loopMap.getLoop(x, y);
        if (!currentLoop) return;

        let newColor: string;
        if (isEraser) {
            newColor = '#FFFFFF'; // белый для ластика
        } else {
            newColor = selectedColor;
        }

        // Оптимизация: не отправляем повторно тот же цвет в ту же клетку
        if (lastSentRef.current?.x === x &&
            lastSentRef.current?.y === y &&
            lastSentRef.current?.color === newColor) {
            return;
        }

        // Обновляем локально
        loopMap.changeLoopColor(x, y, newColor);

        // Создаем новый объект для триггера ререндера
        const updatedMap = new LoopMap(loopMap.m, loopMap.n);
        for (let i = 0; i < loopMap.m; i++) {
            for (let j = 0; j < loopMap.n; j++) {
                const loop = loopMap.getLoop(i, j);
                if (loop) {
                    updatedMap.changeLoopColor(i, j, loop.сolor);
                    updatedMap.changeLoopType(i, j, loop.type);
                }
            }
        }
        setLoopMap(updatedMap);

        // Отправляем на бекенд
        lastSentRef.current = { x, y, color: newColor };

        try {
            await SchemaService.changeLoopColor(x, y, newColor);
            onPixelChange?.();
        } catch (error) {
            console.error('Ошибка сохранения цвета:', error);
        }
    };

    // Обработчики для рисования
    const handleMouseDown = (x: number, y: number) => {
        setIsDrawing(true);
        handlePixelChange(x, y);
    };

    const handleMouseEnter = (x: number, y: number) => {
        if (isDrawing) {
            handlePixelChange(x, y);
        }
    };

    const handleMouseUp = () => {
        setIsDrawing(false);
    };

    // Добавляем глобальный обработчик mouseup
    useEffect(() => {
        window.addEventListener('mouseup', handleMouseUp);
        return () => {
            window.removeEventListener('mouseup', handleMouseUp);
        };
    }, []);

    if (!schemaId) {
        return (
            <div className="schema-field-placeholder">
                <p>📋 Выберите схему</p>
            </div>
        );
    }

    if (!loopMap) {
        return (
            <div className="schema-field-placeholder">
                <p>⏳ Загрузка схемы...</p>
            </div>
        );
    }

    return (

        <div className="schema-field">
            <div className="schema-field">
                {schemaName && (
                    <div className="schema-field-header">
                        <h3>Схема: {schemaName}</h3>
                    </div>
                )}
            </div>
            <div
                className="pixel-grid"
                style={{
                    display: 'grid',
                    gridTemplateColumns: `repeat(${loopMap.n}, 30px)`,
                    gap: '1px',
                    background: '#ccc',
                }}
            >
                {Array.from({ length: loopMap.m }).map((_, x) =>
                    Array.from({ length: loopMap.n }).map((_, y) => {
                        const loop = loopMap.getLoop(x, y);
                        const color = loop?.сolor || '#FFFFFF';
                        return (
                            <div
                                key={`${x}-${y}`}
                                className="pixel"
                                style={{
                                    backgroundColor: color,
                                    width: '30px',
                                    height: '30px',
                                    cursor: 'pointer',
                                }}
                                onMouseDown={() => handleMouseDown(x, y)}
                                onMouseEnter={() => handleMouseEnter(x, y)}
                            />
                        );
                    })
                )}
            </div>
        </div>
    );
};

export default SchemaField;