import { expect, test } from "@playwright/test";

const apiBaseURL = process.env.PLAYWRIGHT_API_URL ?? "http://localhost:5000/api/v1.0";

type CategoriaResponse = {
  id?: string;
  Id?: string;
  descricao?: string;
  Descricao?: string;
};

type PessoaResponse = {
  id?: string;
  Id?: string;
  nome?: string;
  Nome?: string;
};

type TransacaoResponse = {
  id?: string;
  Id?: string;
  descricao?: string;
  Descricao?: string;
  pessoaId?: string;
  PessoaId?: string;
  categoriaId?: string;
  CategoriaId?: string;
};

async function createCategoria(
  request: Parameters<typeof test>[0] extends never ? never : any,
  descricao: string,
  finalidade: 0 | 1 | 2,
) {
  const response = await request.post(`${apiBaseURL}/categorias`, {
    data: { Descricao: descricao, Finalidade: finalidade },
  });

  expect(response.ok()).toBeTruthy();
  const body = (await response.json()) as CategoriaResponse;
  return {
    id: String(body.id ?? body.Id),
    descricao: String(body.descricao ?? body.Descricao ?? descricao),
  };
}

async function createPessoa(
  request: Parameters<typeof test>[0] extends never ? never : any,
  nome: string,
  dataNascimento: string,
) {
  const response = await request.post(`${apiBaseURL}/pessoas`, {
    data: { Nome: nome, DataNascimento: `${dataNascimento}T00:00:00.000Z` },
  });

  expect(response.ok()).toBeTruthy();
  const body = (await response.json()) as PessoaResponse;
  return {
    id: String(body.id ?? body.Id),
    nome: String(body.nome ?? body.Nome ?? nome),
  };
}

async function createTransacao(
  request: Parameters<typeof test>[0] extends never ? never : any,
  data: {
    descricao: string;
    valor: number;
    tipo: 0 | 1;
    categoriaId: string;
    pessoaId: string;
    data: string;
  },
) {
  return request.post(`${apiBaseURL}/transacoes`, {
    data: {
      Descricao: data.descricao,
      Valor: data.valor,
      Tipo: data.tipo,
      CategoriaId: data.categoriaId,
      PessoaId: data.pessoaId,
      Data: `${data.data}T00:00:00.000Z`,
    },
  });
}

async function listTransacoes(request: Parameters<typeof test>[0] extends never ? never : any) {
  const response = await request.get(`${apiBaseURL}/transacoes`, {
    params: { page: "1", pageSize: "200" },
  });

  expect(response.ok()).toBeTruthy();
  const body = await response.json();
  return (body.items ?? body.Items ?? []) as TransacaoResponse[];
}

test.describe("regras de negocio", () => {
  test("menor de idade nao pode registrar receita", async ({ page, request }) => {
    const suffix = Date.now().toString();
    const pessoa = await createPessoa(request, `Menor E2E ${suffix}`, "2012-04-13");
    const categoria = await createCategoria(request, `Categoria Receita Menor ${suffix}`, 1);

    await page.goto("/transacoes");
    await page.getByRole("button", { name: "Adicionar Transação" }).click();
    await expect(page.getByRole("heading", { name: "Adicionar Transação" })).toBeVisible();

    await page.getByLabel("Pessoa").fill(pessoa.nome);
    await expect(page.getByRole("option", { name: pessoa.nome })).toBeVisible();
    await page.getByRole("option", { name: pessoa.nome }).click();

    await expect(page.getByText("Menores só podem registrar despesas.")).toBeVisible();
    await expect(page.getByLabel("Tipo").locator('option[value="receita"]')).toBeDisabled();

    const response = await createTransacao(request, {
      descricao: `Receita proibida ${suffix}`,
      valor: 10,
      tipo: 1,
      categoriaId: categoria.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });

    expect(response.ok()).toBeFalsy();
    const body = await response.json();
    expect(String(body.Detailed ?? body.detailed ?? "")).toContain("Menores de 18 anos não podem registrar receitas.");
  });

  test("categoria respeita finalidade de receita despesa e ambas", async ({ request }) => {
    const suffix = Date.now().toString();
    const pessoa = await createPessoa(request, `Pessoa Finalidade ${suffix}`, "1990-04-13");
    const categoriaDespesa = await createCategoria(request, `Categoria Despesa ${suffix}`, 0);
    const categoriaReceita = await createCategoria(request, `Categoria Receita ${suffix}`, 1);
    const categoriaAmbas = await createCategoria(request, `Categoria Ambas ${suffix}`, 2);

    const receitaEmCategoriaDespesa = await createTransacao(request, {
      descricao: `Receita invalida ${suffix}`,
      valor: 100,
      tipo: 1,
      categoriaId: categoriaDespesa.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });
    expect(receitaEmCategoriaDespesa.ok()).toBeFalsy();
    const bodyReceitaInvalida = await receitaEmCategoriaDespesa.json();
    expect(String(bodyReceitaInvalida.Detailed ?? bodyReceitaInvalida.detailed ?? "")).toContain(
      "Não é possível registrar receita em categoria de despesa.",
    );

    const despesaEmCategoriaReceita = await createTransacao(request, {
      descricao: `Despesa invalida ${suffix}`,
      valor: 50,
      tipo: 0,
      categoriaId: categoriaReceita.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });
    expect(despesaEmCategoriaReceita.ok()).toBeFalsy();
    const bodyDespesaInvalida = await despesaEmCategoriaReceita.json();
    expect(String(bodyDespesaInvalida.Detailed ?? bodyDespesaInvalida.detailed ?? "")).toContain(
      "Não é possível registrar despesa em categoria de receita.",
    );

    const receitaEmCategoriaAmbas = await createTransacao(request, {
      descricao: `Receita valida ${suffix}`,
      valor: 25,
      tipo: 1,
      categoriaId: categoriaAmbas.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });
    expect(receitaEmCategoriaAmbas.ok()).toBeTruthy();

    const despesaEmCategoriaAmbas = await createTransacao(request, {
      descricao: `Despesa valida ${suffix}`,
      valor: 15,
      tipo: 0,
      categoriaId: categoriaAmbas.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });
    expect(despesaEmCategoriaAmbas.ok()).toBeTruthy();
  });

  test("exclui transacoes em cascata ao excluir pessoa", async ({ request }) => {
    const suffix = Date.now().toString();
    const pessoa = await createPessoa(request, `Pessoa Cascade ${suffix}`, "1990-04-13");
    const categoria = await createCategoria(request, `Categoria Cascade ${suffix}`, 2);
    const descricao = `Transacao Cascade ${suffix}`;

    const transacaoResponse = await createTransacao(request, {
      descricao,
      valor: 77.5,
      tipo: 0,
      categoriaId: categoria.id,
      pessoaId: pessoa.id,
      data: "2026-04-13",
    });
    expect(transacaoResponse.ok()).toBeTruthy();

    const antes = await listTransacoes(request);
    expect(
      antes.some((item) => (item.descricao ?? item.Descricao) === descricao && String(item.pessoaId ?? item.PessoaId) === pessoa.id),
    ).toBeTruthy();

    const deleteResponse = await request.delete(`${apiBaseURL}/pessoas/${pessoa.id}`);
    expect(deleteResponse.ok()).toBeTruthy();

    await expect.poll(async () => {
      const pessoasResponse = await request.get(`${apiBaseURL}/pessoas/${pessoa.id}`);
      const transacoes = await listTransacoes(request);
      const transacaoExiste = transacoes.some(
        (item) => (item.descricao ?? item.Descricao) === descricao || String(item.pessoaId ?? item.PessoaId) === pessoa.id,
      );

      return {
        pessoaStatus: pessoasResponse.status(),
        transacaoExiste,
      };
    }).toEqual({
      pessoaStatus: 404,
      transacaoExiste: false,
    });
  });
});
