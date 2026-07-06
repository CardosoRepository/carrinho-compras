import type {
    Carrinho,
    ProblemDetails,
    Produto,
} from "../types/api";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5008";

export class ApiError extends Error {
    public readonly status: number;
    public readonly problemDetails: ProblemDetails | null;

    public constructor(
        message: string,
        status: number,
        problemDetails: ProblemDetails | null,
    ) {
        super(message);

        this.name = "ApiError";
        this.status = status;
        this.problemDetails = problemDetails;
    }
}

async function request<T>(
    path: string,
    options: RequestInit = {},
): Promise<T> {
    const headers = new Headers(options.headers);

    headers.set("Accept", "application/json");

    if (options.body && !headers.has("Content-Type")) {
        headers.set("Content-Type", "application/json");
    }

    const response = await fetch(
        `${API_URL}${path}`,
        {
            ...options,
            headers,
        },
    );

    const responseBody = await readResponseBody(response);

    if (!response.ok) {
        const problemDetails =
            responseBody as ProblemDetails | null;

        const message =
            problemDetails?.detail ??
            problemDetails?.title ??
            "Não foi possível concluir a solicitação.";

        throw new ApiError(
            message,
            response.status,
            problemDetails,
        );
    }

    return responseBody as T;
}

async function readResponseBody(response: Response): Promise<unknown> {
    if (response.status === 204) {
        return null;
    }

    const contentType =
        response.headers.get("content-type");

    if (!contentType?.includes("application/json")) {
        return null;
    }

    return response.json();
}

export function listarProdutos(): Promise<Produto[]> {
    return request<Produto[]>("/api/produtos");
}

export function criarCarrinho(): Promise<Carrinho> {
    return request<Carrinho>(
        "/api/carrinhos",
        {
            method: "POST",
        },
    );
}

export function obterCarrinho(carrinhoId: string): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}`,
    );
}

export function adicionarItem(
    carrinhoId: string,
    produtoId: string,
    quantidade = 1,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/itens`,
        {
            method: "POST",
            body: JSON.stringify({
                produtoId,
                quantidade,
            }),
        },
    );
}

export function alterarQuantidadeItem(
    carrinhoId: string,
    produtoId: string,
    quantidade: number,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/itens/${produtoId}`,
        {
            method: "PUT",
            body: JSON.stringify({
                quantidade,
            }),
        },
    );
}

export function removerItem(
    carrinhoId: string,
    produtoId: string,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/itens/${produtoId}`,
        {
            method: "DELETE",
        },
    );
}

export function aplicarCupom(
    carrinhoId: string,
    codigoCupom: string,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/cupom`,
        {
            method: "PUT",
            body: JSON.stringify({
                codigoCupom,
            }),
        },
    );
}

export function removerCupom(
    carrinhoId: string,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/cupom`,
        {
            method: "DELETE",
        },
    );
}

export function finalizarCarrinho(
    carrinhoId: string,
): Promise<Carrinho> {
    return request<Carrinho>(
        `/api/carrinhos/${carrinhoId}/finalizar`,
        {
            method: "POST",
        },
    );
}