import type { Carrinho, ItemCarrinho } from "../types/api";
import { AcoesCarrinho } from "./AcoesCarrinho";
import { formatarMoeda } from "../utils/formatadores";

interface ResumoCarrinhoProps {
  carrinho: Carrinho | null;
  criandoCarrinho: boolean;
  produtoEmProcessamento: string | null;
  processandoCupom: boolean;
  finalizandoCarrinho: boolean;
  onCriarCarrinho: () => void;
  onCriarNovoCarrinho: () => void;
  onAlterarQuantidade: (produtoId: string, quantidade: number) => void;
  onRemoverItem: (produtoId: string) => void;
  onAplicarCupom: (codigoCupom: string) => Promise<boolean>;
  onRemoverCupom: () => void;
  onFinalizarCarrinho: () => void;
}

export function ResumoCarrinho({
  carrinho,
  criandoCarrinho,
  produtoEmProcessamento,
  processandoCupom,
  finalizandoCarrinho,
  onCriarCarrinho,
  onCriarNovoCarrinho,
  onAlterarQuantidade,
  onRemoverItem,
  onAplicarCupom,
  onRemoverCupom,
  onFinalizarCarrinho,
}: ResumoCarrinhoProps) {
  if (!carrinho) {
    return (
      <aside className="resumo-carrinho">
        <h2>Seu carrinho</h2>

        <div className="carrinho-vazio">
          <p>Inicie um carrinho para adicionar produtos.</p>

          <button
            className="botao botao--primario botao--largura-total"
            type="button"
            disabled={criandoCarrinho}
            onClick={onCriarCarrinho}
          >
            {criandoCarrinho ? "Criando..." : "Iniciar carrinho"}
          </button>
        </div>
      </aside>
    );
  }

  const carrinhoAberto = carrinho.status === "Aberto";

  return (
    <aside className="resumo-carrinho">
      <div className="resumo-carrinho__titulo">
        <h2>Seu carrinho</h2>

        <span
          className={
            carrinhoAberto
              ? "status status--aberto"
              : "status status--finalizado"
          }
        >
          {carrinho.status}
        </span>
      </div>

      <div className="carrinho-identificacao">
        <span>Identificador</span>

        <small title={carrinho.id}>{carrinho.id}</small>
      </div>

      {carrinho.itens.length === 0 ? (
        <div className="carrinho-itens-vazio">Nenhum produto adicionado.</div>
      ) : (
        <div className="lista-itens">
          {carrinho.itens.map((item) => (
            <ItemDoCarrinho
              key={item.produtoId}
              item={item}
              carrinhoAberto={carrinhoAberto}
              processando={produtoEmProcessamento === item.produtoId}
              onAlterarQuantidade={onAlterarQuantidade}
              onRemoverItem={onRemoverItem}
            />
          ))}
        </div>
      )}

      <div className="totais">
        <div>
          <span>Subtotal</span>

          <strong>{formatarMoeda.format(carrinho.subtotal)}</strong>
        </div>

        <div>
          <span>Desconto</span>

          <strong>{formatarMoeda.format(carrinho.desconto)}</strong>
        </div>

        <div className="totais__total">
          <span>Total</span>

          <strong>{formatarMoeda.format(carrinho.total)}</strong>
        </div>
      </div>
      
      <AcoesCarrinho
        carrinho={carrinho}
        processandoCupom={processandoCupom}
        finalizandoCarrinho={finalizandoCarrinho}
        criandoCarrinho={criandoCarrinho}
        operacaoDeItemEmAndamento={produtoEmProcessamento !== null}
        onAplicarCupom={onAplicarCupom}
        onRemoverCupom={onRemoverCupom}
        onFinalizarCarrinho={onFinalizarCarrinho}
        onCriarNovoCarrinho={onCriarNovoCarrinho}
      />
    </aside>
  );
}

interface ItemDoCarrinhoProps {
  item: ItemCarrinho;
  carrinhoAberto: boolean;
  processando: boolean;
  onAlterarQuantidade: (produtoId: string, quantidade: number) => void;
  onRemoverItem: (produtoId: string) => void;
}

function ItemDoCarrinho({
  item,
  carrinhoAberto,
  processando,
  onAlterarQuantidade,
  onRemoverItem,
}: ItemDoCarrinhoProps) {
  const podeDiminuir = carrinhoAberto && !processando && item.quantidade > 1;

  const podeAumentar =
    carrinhoAberto &&
    !processando &&
    item.quantidade < item.quantidadeDisponivelEstoque;

  return (
    <article className="item-carrinho">
      <div className="item-carrinho__cabecalho">
        <div>
          <h3>{item.descricaoProduto}</h3>

          <small>
            {formatarMoeda.format(item.precoLiquidoUnitario)} por unidade
          </small>
        </div>

        <strong>{formatarMoeda.format(item.precoTotal)}</strong>
      </div>

      <div className="item-carrinho__acoes">
        <div
          className="controle-quantidade"
          aria-label={`Quantidade de ${item.descricaoProduto}`}
        >
          <button
            type="button"
            aria-label={`Diminuir quantidade de ${item.descricaoProduto}`}
            disabled={!podeDiminuir}
            onClick={() =>
              onAlterarQuantidade(item.produtoId, item.quantidade - 1)
            }
          >
            −
          </button>

          <span>{item.quantidade}</span>

          <button
            type="button"
            aria-label={`Aumentar quantidade de ${item.descricaoProduto}`}
            disabled={!podeAumentar}
            onClick={() =>
              onAlterarQuantidade(item.produtoId, item.quantidade + 1)
            }
          >
            +
          </button>
        </div>

        <button
          className="botao-remover"
          type="button"
          disabled={!carrinhoAberto || processando}
          onClick={() => onRemoverItem(item.produtoId)}
        >
          {processando ? "Processando..." : "Remover"}
        </button>
      </div>
    </article>
  );
}
