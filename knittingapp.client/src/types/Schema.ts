
export interface Loop {
    m: number;
    n: number;
    color: string;
    type: string;
}


export interface Schema {
    schemaId?: string;
    schemaName: string;
    loopMap: LoopMap;
    schemaImage?: string;
}

export class LoopMap {
    m: number;
    n: number;
    loopMap: Loop[][];
    loopWidth?: number;
    loopHeight?: number;
    nullM?: number;
    nullN?: number;
    constructor(m: number, n: number, loopWidth?: number, loopHeight?: number, nullM?:number, nullN?:number) {
        this.m = m;
        this.n = n;
        this.loopHeight = loopHeight;
        this.loopWidth = loopWidth;
        this.nullM = nullM;
        this.nullN = nullN;

        // Создаём матрицу с петлями по умолчанию
        this.loopMap = Array(m).fill(null).map((_, x) =>
            Array(n).fill(null).map((_, y) => ({
                m: x,           // можно использовать координаты
                n: y,
                color: "#FFFFFF",  // белый цвет по умолчанию
                type: "loop"           // тип loop по умолчанию
            }))
        );
    }

    // Получить значение из ячейки
    getLoop(x: number, y: number): Loop | null {
        if (x >= 0 && x < this.m && y >= 0 && y < this.n) {
            return this.loopMap[x][y];
        }
        return null;
    }

    // Обновить значение в ячейке
    changeLoopColor(x: number, y: number, color:string): void {
        if (x >= 0 && x < this.m && y >= 0 && y < this.n) {
            this.loopMap[x][y].color = color;
        }
    }

    changeLoopType(x: number, y: number, type: string): void {
        if (x >= 0 && x < this.m && y >= 0 && y < this.n) {
            this.loopMap[x][y].type = type;
        }
    }
}
