import ColorCircle from "../components/Schema/ColorCircle";
import CreateSchemaForm from '../components/Schema/CreateSchemaForm';
import SchemaField from '../components/Schema/SchemaFiled';
//import type { ColorCircleValue } from "../components/Schema/ColorCircle";
import type { Schema } from '../types/Schema';
import { SchemaService } from '../services/SchemaService';
import { useState, useEffect } from 'react';
import '../styles/SchemaView.css'

interface ColorCircleValue {
    color: string;
    isEraser: boolean;
}
function SchemaConstructorView() {
        const [schemas, setSchemas] = useState<Schema[]>([]);
        const [selectedSchemaId, setSelectedSchemaId] = useState<string | null>(null);
        const [isModalOpen, setIsModalOpen] = useState(false);
        const [colorCircleValue, setColorCircleValue] = useState<ColorCircleValue>({
            color: '#ff3b30',
            isEraser: false
        });

        // Загрузка списка схем
        const loadSchemas = async () => {
            try {
                const data = await SchemaService.getAllSchemas();
                setSchemas(data);
            } catch (error) {
                console.error('Ошибка загрузки схем:', error);
            }
        };

        useEffect(() => {
            const fetchSchemas = async () => {  
                await loadSchemas();            
            };
            fetchSchemas(); 
        }, []);

        // Создание новой схемы
        const handleCreateSchema = async (name: string, m: number, n: number) => {
            await SchemaService.createSchema(m, n, name);
            await loadSchemas(); // Обновляем список
        };

        // Выбор схемы
        const handleSelectSchema = (schemaId: string) => {
            setSelectedSchemaId(schemaId);
        };

        // Обновление после изменения пикселя
        const handlePixelChange = () => {
    };

    // Функция для получения правильного src
    const getImageSrc = (imageData: string | undefined) => {
        if (!imageData) return '';

        // Если уже есть data:image префикс
        if (imageData.startsWith('data:image')) {
            return imageData;
        }

        // Если начинается с iVBOR (PNG signature)
        if (imageData.startsWith('iVBOR')) {
            return `data:image/png;base64,${imageData}`;
        }

        // Если просто base64 без префикса
        return `data:image/png;base64,${imageData}`;
    };


        return (
            <div className="schema-constructor-view">
                <div className="left-panel">
                    <div className="schemas-header">
                        <h2>Схемы</h2>
                        <button
                            className="create-schema-btn"
                            onClick={() => setIsModalOpen(true)}
                        >
                            + Создать схему
                        </button>
                    </div>

                    <div className="schemas-list">

                        {schemas.length === 0 ? (
                            <div className="no-schemas">Схем не найдено</div>
                        ) : (
                                schemas.map((schema) => {
                                   // console.log(schema);
                                    return (<button
                                        key={schema.schemaId}
                                        className={`schema-btn ${selectedSchemaId === schema.schemaId ? 'active' : ''}`}
                                        onClick={() => handleSelectSchema(schema.schemaId!)}
                                    >
                                        <div className="schema-name">{schema.schemaName}</div>
                                        {/*<div className="schema-size">
                                        {schema.loopMap.m} × {schema.loopMap.n}
                                    </div>*/}
                                        {schema.schemaImage && (
                                            <div className="schema-preview">
                                                <img
                                                    src={getImageSrc(schema.schemaImage)}
                                                    alt={schema.schemaName}
                                                    className="schema-preview-img"
                                                    onError={(e) => {
                                                        console.error('Ошибка загрузки картинки:', schema.schemaImage?.substring(0, 50));
                                                        e.currentTarget.style.display = 'none';
                                                    }}
                                                />
                                            </div>
                                        )}
                                    </button>)
                                })
                        )}
                    </div>
                </div>

                <div className="right-panel">
                    <div className="color-picker-section">
                        <ColorCircle
                            value={colorCircleValue}
                            onChange={setColorCircleValue}
                        />
                    </div>

                    <div className="schema-field-section">
                        <SchemaField
                            schemaId={selectedSchemaId}
                            selectedColor={colorCircleValue.color}
                            isEraser={colorCircleValue.isEraser}
                            onPixelChange={handlePixelChange}
                        />
                    </div>
                </div>

                <CreateSchemaForm
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    onCreate={handleCreateSchema}
                />
            </div>
        );
    }

export default SchemaConstructorView;