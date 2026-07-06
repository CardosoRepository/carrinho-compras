import type { Produto } from "../types/api";
import { formatarMoeda } from "../utils/formatadores";

interface ProdutoCardProps {
  produto: Produto;
  quantidadeNoCarrinho: number;
  processando: boolean;
  desabilitado: boolean;
  onAdicionar: (produtoId: string) => void;
}

export function ProdutoCard({
  produto,
  quantidadeNoCarrinho,
  processando,
  desabilitado,
  onAdicionar,
}: ProdutoCardProps) {
  const semEstoque = produto.quantidadeEstoque === 0;

  return (
    <article className="produto-card">
      <div className="produto-card__conteudo">
        <h3>{produto.descricaoProduto}</h3>

        <p>{produto.quantidadeEstoque} unidades disponíveis</p>

        {quantidadeNoCarrinho > 0 && (
          <span className="produto-card__quantidade">
            No carrinho: {quantidadeNoCarrinho}
          </span>
        )}
      </div>

      <div className="produto-card__rodape">
        <strong className="produto-card__preco">
          {formatarMoeda.format(produto.precoLiquido)}
        </strong>

        <button
          className="botao botao--primario"
          type="button"
          disabled={desabilitado || processando || semEstoque}
          onClick={() => onAdicionar(produto.id)}
        >
          {processando
            ? "Adicionando..."
            : quantidadeNoCarrinho > 0
              ? "Adicionar mais"
              : "Adicionar"}
        </button>
      </div>
    </article>
  );
}
