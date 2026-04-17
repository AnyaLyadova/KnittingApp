import { useState } from 'react';
import { HexColorPicker } from 'react-colorful';

export interface ColorCircleValue {
    color: string;
    isEraser: boolean;
}

interface ColorCircleProps {
    value: ColorCircleValue;
    onChange: (value: ColorCircleValue) => void;
}

// Базовые цвета палитры
const BASE_PALETTE: string[] = [
    '#000000', '#ff3b30', '#ff9500', '#ffcc00',
    '#34c759', '#007aff', '#5856d6', '#af52de'
];

const ColorCircle: React.FC<ColorCircleProps> = ({ value, onChange }) => {
    const [customColors, setCustomColors] = useState<string[]>([]);

    const allColors: string[] = [...BASE_PALETTE, ...customColors];

    const addCustomColor = (): void => {
        if (!customColors.includes(value.color)) {
            setCustomColors([...customColors, value.color]);
        }
    };

    // Обработчик выбора цвета
    const handleColorSelect = (color: string): void => {
        onChange({ color, isEraser: false });
    };

    // Обработчик выбора ластика
    const handleEraserClick = (): void => {
        onChange({ color: '#ffffff', isEraser: true });
    };

    return (
        <div className="color-picker">
            {/* Цветовое колесо */}
            <div className="wheel-container">
                <h3>Цветовой круг</h3>
                <HexColorPicker
                    color={value.isEraser ? '#ffffff' : value.color}
                    onChange={handleColorSelect}
                />
                <button
                    onClick={addCustomColor}
                    className="add-btn"
                    disabled={value.isEraser}
                >
                    + Добавить в палитру
                </button>
            </div>

            {/* Палитра цветов */}
            <div className="palette-container">
                <h3> Палитра</h3>

                {/* Ластик */}
                <div className="eraser-section">
                    <div
                        className={`eraser-swatch ${value.isEraser ? 'active' : ''}`}
                        onClick={handleEraserClick}
                    >
                       Ластик
                    </div>
                    <span className="eraser-label">Ластик</span>
                </div>

                {/* Цвета палитры */}
                <div className="palette-grid">
                    {allColors.map((color, idx) => (
                        <div
                            key={idx}
                            className={`color-swatch ${!value.isEraser && value.color === color ? 'active' : ''}`}
                            style={{ backgroundColor: color }}
                            onClick={() => handleColorSelect(color)}
                        />
                    ))}
                </div>

                {customColors.length > 0 && (
                    <button
                        onClick={() => setCustomColors([])}
                        className="clear-btn"
                    >
                        Очистить добавленные
                    </button>
                )}
            </div>
        </div>
    );
};


export default ColorCircle;