import { useEffect, useState } from "react";
import type { Schema } from "../../types/Schema";
import { SchemaService } from "../../services/SchemaService";

interface SchemasListProps {
    // onSelectSchema: (schemaId: string) => void;     
    onSelectSchema: (schema: Schema) => void;
}

function SchemasList({onSelectSchema}: SchemasListProps) {
    const [schemas, setSchemas] = useState<Schema[]>([]);
    const [selectedSchema, setSelectedSchema] = useState<Schema | null>(null);


    // Загрузка списка схем
    const loadSchemas = async () => {
        try {
            const data = await SchemaService.getAllSchemas();
            setSchemas(data);
        } catch (error) {
            console.error('Ошибка загрузки схем:', error);
        }
    };

    // Выбор схемы
    const handleSelectSchema = (schema: Schema) => {
        setSelectedSchema(schema);
        onSelectSchema(schema);
    };

    useEffect(() => {
        const fetchSchemas = async () => {
            await loadSchemas();
        };
        fetchSchemas();
    }, []);

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

    return(
    <div className="schemas-list">

        {schemas.length === 0 ? (
            <div className="no-schemas">Схем не найдено</div>
        ) : (
            schemas.map((schema) => {
                // console.log(schema);
                return (<button
                    key={schema.schemaId}
                    className={`schema-btn ${selectedSchema === schema ? 'active' : ''}`}
                    onClick={() => handleSelectSchema(schema)}
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
    </div>)

}

export default SchemasList;