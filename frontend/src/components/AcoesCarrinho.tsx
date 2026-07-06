import { useState } from "react";

import type { FormEvent } from "react";

import type { Carrinho } from "../types/api";

interface AcoesCarrinhoProps {
  carrinho: Carrinho;
  processandoCupom: boolean;
  finalizandoCarrinho: boolean;
  criandoCarrinho: boolean;
  operacaoEmAndamento: boolean;
  onAplicarCupom: (codigoCupom: string) => Promise<boolean>;
  onRemoverCupom: () => void;
  onFinalizarCarrinho: () => void;
  onCriarNovoCarrinho: () => void;
}

export function AcoesCarrinho({
  carrinho,
  processandoCupom,
  finalizandoCarrinho,
  criandoCarrinho,
  operacaoEmAndamento,
  onAplicarCupom,
  onRemoverCupom,
  onFinalizarCarrinho,
  onCriarNovoCarrinho,
}: AcoesCarrinhoProps) {
  const [codigoCupom, setCodigoCupom] = useState("");

  const carrinhoAberto = carrinho.status === "Aberto";

  async function aplicarCodigo() {
    const codigoNormalizado = codigoCupom.trim();

    if (!codigoNormalizado) {
      return;
    }

    const aplicado = await onAplicarCupom(codigoNormalizado);

    if (aplicado) {
      setCodigoCupom("");
    }
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    void aplicarCodigo();
  }

  if (!carrinhoAberto) {
    return (
      <section className="acoes-carrinho">
        <div className="carrinho-finalizado">
          <strong>Carrinho finalizado</strong>

          <p>Este carrinho não pode mais ser alterado.</p>
        </div>

        <button
          className="botao botao--secundario botao--largura-total"
          type="button"
          disabled={operacaoEmAndamento}
          onClick={onCriarNovoCarrinho}
        >
          {criandoCarrinho ? "Criando..." : "Criar novo carrinho"}
        </button>
      </section>
    );
  }

  const cupomAplicado = carrinho.cupomAplicado;

  return (
    <section className="acoes-carrinho">
      <div className="cupom">
        <div className="cupom__titulo">
          <h3>Cupom de desconto</h3>

          {cupomAplicado && (
            <button
              className="botao-remover"
              type="button"
              disabled={operacaoEmAndamento}
              onClick={onRemoverCupom}
            >
              {processandoCupom ? "Processando..." : "Remover cupom"}
            </button>
          )}
        </div>

        {cupomAplicado && (
          <div className="cupom-aplicado">
            <div>
              <strong>{cupomAplicado.codigoCupom}</strong>

              <span>{cupomAplicado.percentualDesconto}% de desconto</span>
            </div>

            <span className="cupom-aplicado__status">Aplicado</span>
          </div>
        )}

        <form className="cupom-form" onSubmit={handleSubmit}>
          <label htmlFor="codigoCupom">
            {cupomAplicado ? "Trocar cupom" : "Adicionar cupom"}
          </label>

          <div className="cupom-form__linha">
            <input
              id="codigoCupom"
              name="codigoCupom"
              type="text"
              value={codigoCupom}
              maxLength={20}
              placeholder="10OFF ou 15OFF"
              disabled={operacaoEmAndamento}
              onChange={(event) => setCodigoCupom(event.target.value)}
            />

            <button
              className="botao botao--secundario"
              type="submit"
              disabled={operacaoEmAndamento || !codigoCupom.trim()}
            >
              {processandoCupom
                ? "Aplicando..."
                : cupomAplicado
                  ? "Trocar"
                  : "Aplicar"}
            </button>
          </div>
        </form>
      </div>

      <button
        className="botao botao--primario botao--largura-total"
        type="button"
        disabled={operacaoEmAndamento}
        onClick={onFinalizarCarrinho}
      >
        {finalizandoCarrinho ? "Finalizando..." : "Finalizar carrinho"}
      </button>
    </section>
  );
}
