export interface Model {
    modelId: string;
    name: string;
}


export interface NeckParts {  //типы горловины
    [key: string]: string;
}

export interface SleeveRollParts {  //типы скоса рукава
    [key: string]: string;
}

export interface ArmholeParts {  //типы выемки рукава
    [key: string]: string;
}

export interface Measures {
    [key:string]:number
}

/*export interface CreateModelRequest {
    name: string;
    stringParts: string[];  // массив названий выбранных частей
}

export interface CreateModelResponse {
    name: string;
    parts: string[];
}*/
