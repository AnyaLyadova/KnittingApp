
export interface Point {
    x: number;
    y: number;
    part?: string;
    visible: boolean;
}

export interface Draft {
    draftId:string
    draft: Point[];

}
