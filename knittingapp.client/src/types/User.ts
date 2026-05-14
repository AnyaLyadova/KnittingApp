export interface User {
    token?: string;
    user?: string;
    login?: string;
}

export interface ApiResponse {
    ok: boolean;
    status: number;
    data?: User | { message: string };
    error?: string;
}