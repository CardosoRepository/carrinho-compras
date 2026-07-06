import { useEffect, useRef, useState } from "react";

import { ProdutoCard } from "./components/ProdutoCard";
import { ResumoCarrinho } from "./components/ResumoCarrinho";

import {
  adicionarItem,
  alterarQuantidadeItem,
  ApiError,
  aplicarCupom,
  criarCarrinho,
  finalizarCarrinho,
  listarProdutos,
  obterCarrinho,
  removerCupom,
  removerItem,
} from "./services/api";

import type { Carrinho, Produto } from "./types/api";

const CARRINHO_STORAGE_KEY = "carrinho-compras:carrinho-id";

function App() {
  const [produtos, setProdutos] = useState<Produto[]>([]);

  const [carrinho, setCarrinho] = useState<Carrinho | null>(null);

  const [carregando, setCarregando] = useState(true);

  const [criandoCarrinho, setCriandoCarrinho] = useState(false);

  const [produtoEmProcessamento, setProdutoEmProcessamento] = useState<
    string | null
  >(null);

  const [processandoCupom, setProcessandoCupom] = useState(false);

  const [finalizandoCarrinho, setFinalizandoCarrinho] = useState(false);

  const [erro, setErro] = useState<string | null>(null);

  const operacaoEmAndamentoRef = useRef(false);

  const operacaoEmAndamento =
    criandoCarrinho ||
    produtoEmProcessamento !== null ||
    processandoCupom ||
    finalizandoCarrinho;

  useEffect(() => {
    let ignorarResultado = false;

    async function carregarDadosIniciais() {
      try {
        const produtosResponse = await listarProdutos();

        if (ignorarResultado) {
          return;
        }

        setProdutos(produtosResponse);

        const carrinhoId = localStorage.getItem(CARRINHO_STORAGE_KEY);

        if (!carrinhoId) {
          return;
        }

        try {
          const carrinhoResponse = await obterCarrinho(carrinhoId);

          if (!ignorarResultado) {
            setCarrinho(carrinhoResponse);
          }
        } catch (error) {
          if (error instanceof ApiError && error.status === 404) {
            localStorage.removeItem(CARRINHO_STORAGE_KEY);

            return;
          }

          throw error;
        }
      } catch (error) {
        if (!ignorarResultado) {
          setErro(obterMensagemErro(error));
        }
      } finally {
        if (!ignorarResultado) {
          setCarregando(false);
        }
      }
    }

    void carregarDadosIniciais();

    return () => {
      ignorarResultado = true;
    };
  }, []);

  async function criarESalvarCarrinho() {
    const carrinhoCriado = await criarCarrinho();

    setCarrinho(carrinhoCriado);

    localStorage.setItem(CARRINHO_STORAGE_KEY, carrinhoCriado.id);

    return carrinhoCriado;
  }

  function tentarIniciarOperacao(): boolean {
    if (operacaoEmAndamentoRef.current) {
      return false;
    }

    operacaoEmAndamentoRef.current = true;

    return true;
  }

  function encerrarOperacao() {
    operacaoEmAndamentoRef.current = false;
  }

  async function handleCriarCarrinho() {
    if (!tentarIniciarOperacao()) {
      return;
    }

    setCriandoCarrinho(true);
    setErro(null);

    try {
      await criarESalvarCarrinho();
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setCriandoCarrinho(false);
      encerrarOperacao();
    }
  }

  async function handleAdicionarProduto(produtoId: string) {
    if (!tentarIniciarOperacao()) {
      return;
    }

    setProdutoEmProcessamento(produtoId);
    setErro(null);

    try {
      const carrinhoAtual = carrinho ?? (await criarESalvarCarrinho());

      const carrinhoAtualizado = await adicionarItem(
        carrinhoAtual.id,
        produtoId,
        1,
      );

      setCarrinho(carrinhoAtualizado);
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setProdutoEmProcessamento(null);
      encerrarOperacao();
    }
  }

  async function handleAlterarQuantidade(
    produtoId: string,
    quantidade: number,
  ) {
    if (!carrinho || !tentarIniciarOperacao()) {
      return;
    }

    setProdutoEmProcessamento(produtoId);
    setErro(null);

    try {
      const carrinhoAtualizado = await alterarQuantidadeItem(
        carrinho.id,
        produtoId,
        quantidade,
      );

      setCarrinho(carrinhoAtualizado);
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setProdutoEmProcessamento(null);
      encerrarOperacao();
    }
  }

  async function handleRemoverItem(produtoId: string) {
    if (!carrinho || !tentarIniciarOperacao()) {
      return;
    }

    setProdutoEmProcessamento(produtoId);
    setErro(null);

    try {
      const carrinhoAtualizado = await removerItem(carrinho.id, produtoId);

      setCarrinho(carrinhoAtualizado);
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setProdutoEmProcessamento(null);
      encerrarOperacao();
    }
  }

  async function handleAplicarCupom(codigoCupom: string): Promise<boolean> {
    if (!carrinho || !tentarIniciarOperacao()) {
      return false;
    }

    setProcessandoCupom(true);
    setErro(null);

    try {
      const carrinhoAtualizado = await aplicarCupom(carrinho.id, codigoCupom);

      setCarrinho(carrinhoAtualizado);

      return true;
    } catch (error) {
      setErro(obterMensagemErro(error));

      return false;
    } finally {
      setProcessandoCupom(false);
      encerrarOperacao();
    }
  }

  async function handleRemoverCupom() {
    if (!carrinho || !tentarIniciarOperacao()) {
      return;
    }

    setProcessandoCupom(true);
    setErro(null);

    try {
      const carrinhoAtualizado = await removerCupom(carrinho.id);

      setCarrinho(carrinhoAtualizado);
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setProcessandoCupom(false);
      encerrarOperacao();
    }
  }

  async function handleFinalizarCarrinho() {
    if (!carrinho || !tentarIniciarOperacao()) {
      return;
    }

    setFinalizandoCarrinho(true);
    setErro(null);

    try {
      const carrinhoAtualizado = await finalizarCarrinho(carrinho.id);

      setCarrinho(carrinhoAtualizado);
    } catch (error) {
      setErro(obterMensagemErro(error));
    } finally {
      setFinalizandoCarrinho(false);
      encerrarOperacao();
    }
  }

  if (carregando) {
    return (
      <main className="pagina-centralizada">
        <p>Carregando produtos...</p>
      </main>
    );
  }

  const carrinhoAberto = carrinho?.status === "Aberto";

  return (
    <div className="aplicacao">
      <header className="cabecalho">
        <div>
          <span className="cabecalho__etiqueta">Desafio técnico</span>

          <h1>Carrinho de compras</h1>

          <p>Escolha os produtos e acompanhe os valores do seu carrinho.</p>
        </div>

        {!carrinho && (
          <button
            className="botao botao--primario"
            type="button"
            disabled={operacaoEmAndamento}
            onClick={handleCriarCarrinho}
          >
            {criandoCarrinho ? "Criando..." : "Iniciar carrinho"}
          </button>
        )}
      </header>

      {erro && (
        <div className="mensagem mensagem--erro" role="alert">
          {erro}
        </div>
      )}

      <main className="conteudo">
        <section className="produtos">
          <div className="secao-titulo">
            <div>
              <span>Catálogo</span>
              <h2>Produtos disponíveis</h2>
            </div>

            <strong>{produtos.length} produtos</strong>
          </div>

          <div className="grade-produtos">
            {produtos.map((produto) => {
              const itemNoCarrinho = carrinho?.itens.find(
                (item) => item.produtoId === produto.id,
              );

              return (
                <ProdutoCard
                  key={produto.id}
                  produto={produto}
                  quantidadeNoCarrinho={itemNoCarrinho?.quantidade ?? 0}
                  processando={produtoEmProcessamento === produto.id}
                  desabilitado={
                    operacaoEmAndamento ||
                    (carrinho !== null && !carrinhoAberto)
                  }
                  onAdicionar={handleAdicionarProduto}
                />
              );
            })}
          </div>
        </section>

        <ResumoCarrinho
          carrinho={carrinho}
          criandoCarrinho={criandoCarrinho}
          operacaoEmAndamento={operacaoEmAndamento}
          produtoEmProcessamento={produtoEmProcessamento}
          processandoCupom={processandoCupom}
          finalizandoCarrinho={finalizandoCarrinho}
          onCriarCarrinho={handleCriarCarrinho}
          onCriarNovoCarrinho={handleCriarCarrinho}
          onAlterarQuantidade={handleAlterarQuantidade}
          onRemoverItem={handleRemoverItem}
          onAplicarCupom={handleAplicarCupom}
          onRemoverCupom={handleRemoverCupom}
          onFinalizarCarrinho={handleFinalizarCarrinho}
        />
      </main>
    </div>
  );
}

function obterMensagemErro(error: unknown): string {
  if (error instanceof ApiError) {
    return error.message;
  }

  return "Ocorreu um erro inesperado.";
}

export default App;
