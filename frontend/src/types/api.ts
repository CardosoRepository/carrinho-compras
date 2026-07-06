export interface Produto {
    id: string;
    descricaoProduto: string;
    quantidadeEstoque: number;
    precoLiquido: number;
}

export interface ItemCarrinho {
    produtoId: string;
    descricaoProduto: string;
    quantidade: number;
    precoLiquidoUnitario: number;
    precoTotal: number;
    quantidadeDisponivelEstoque: number;
}

export interface CupomAplicado {
    id: string;
    codigoCupom: string;
    percentualDesconto: number;
}

export interface Carrinho {
    id: string;
    status: string;
    itens: ItemCarrinho[];
    cupomAplicado: CupomAplicado | null;
    subtotal: number;
    desconto: number;
    total: number;
}

export interface ProblemDetails {
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    traceId?: string;
    errors?: Record<string, string[]>;
}