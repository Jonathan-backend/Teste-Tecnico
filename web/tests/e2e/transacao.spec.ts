import { expect, test } from "@playwright/test";

const apiBaseURL = process.env.PLAYWRIGHT_API_URL ?? "http://localhost:5000/api/v1.0";

test("deve criar transacao e refletir em transacoes e relatorios", async ({ page, request }) => {
  const suffix = Date.now().toString();
  const categoria = `Receita E2E ${suffix}`;
  const pessoa = `Pessoa Fluxo ${suffix}`;
  const descricao = `Transacao Fluxo ${suffix}`;
  const data = "2026-04-13";

  
  await page.goto("/categorias");
  await page.getByRole("button", { name: "Adicionar Categoria" }).click();
  await expect(page.getByRole("heading", { name: "Adicionar Categoria" })).toBeVisible();
  await page.getByLabel("Descrição").fill(categoria);
  await page.getByLabel("Finalidade").selectOption("receita");
  const categoriaResponsePromise = page.waitForResponse((response) =>
    response.request().method() === "POST" && response.url().includes("/categorias"),
  );
  await page.getByRole("button", { name: "Salvar" }).click();
  const categoriaResponse = await categoriaResponsePromise;
  expect(categoriaResponse.ok()).toBeTruthy();

  
  await page.goto("/pessoas");
  await page.getByRole("button", { name: "Adicionar Pessoa" }).click();
  await expect(page.getByRole("heading", { name: "Adicionar Pessoa" })).toBeVisible();
  await page.getByLabel("Nome").fill(pessoa);
  await page.getByLabel("Data de Nascimento").fill("1990-04-13");
  const pessoaResponsePromise = page.waitForResponse((response) =>
    response.request().method() === "POST" && response.url().includes("/pessoas"),
  );
  await page.getByRole("button", { name: "Salvar" }).click();
  const pessoaResponse = await pessoaResponsePromise;
  expect(pessoaResponse.ok()).toBeTruthy();

  
  await page.goto("/transacoes");
  await page.getByRole("button", { name: "Adicionar Transação" }).click();
  await expect(page.getByRole("heading", { name: "Adicionar Transação" })).toBeVisible();

  await page.getByLabel("Descrição").fill(descricao);
  await page.getByLabel("Valor").fill("1234.56");
  await page.getByLabel("Data").fill(data);
  await page.getByLabel("Tipo").selectOption("receita");

  await page.getByLabel("Pessoa").fill(pessoa);
  await expect(page.getByRole("option", { name: pessoa })).toBeVisible();
  await page.getByRole("option", { name: pessoa }).click();

  await page.getByLabel("Categoria").fill(categoria);
  await expect(page.getByRole("option", { name: categoria })).toBeVisible();
  await page.getByRole("option", { name: categoria }).click();

  const transacaoResponsePromise = page.waitForResponse((response) =>
    response.request().method() === "POST" && response.url().includes("/transacoes"),
  );
  await page.getByRole("button", { name: "Salvar" }).click();
  const transacaoResponse = await transacaoResponsePromise;
  expect(transacaoResponse.ok()).toBeTruthy();
  const transacaoCriada = await transacaoResponse.json();
  const pessoaId = String(transacaoCriada.pessoaId ?? transacaoCriada.PessoaId);

  
  await page.goto("/totais");
  await expect(page.getByRole("heading", { name: "Totais por Pessoa" })).toBeVisible();

  await expect.poll(async () => {
    const response = await request.get(`${apiBaseURL}/transacoes`, {
      params: { page: "1", pageSize: "200" },
    });
    const body = await response.json();
    return body.items?.some(
      (item: { descricao?: string; Descricao?: string; pessoaNome?: string; PessoaNome?: string }) =>
        (item.descricao ?? item.Descricao) === descricao && (item.pessoaNome ?? item.PessoaNome) === pessoa,
    );
  }).toBeTruthy();

  await expect.poll(async () => {
    const response = await request.get(`${apiBaseURL}/Totais/pessoas`, {
      params: {
        page: "1",
        pageSize: "50",
        "Pessoa.Id": pessoaId,
        "Periodo.DataInicio": `${data}T00:00:00`,
        "Periodo.DataFim": `${data}T23:59:59`,
      },
    });
    const body = await response.json();
    const total = body.items?.find(
      (item: { pessoaId?: string; PessoaId?: string }) => String(item.pessoaId ?? item.PessoaId) === pessoaId,
    );
    const receitas = Number(total?.totalReceitas ?? total?.TotalReceitas ?? 0);
    return total && receitas === 1234.56;
  }).toBeTruthy();
});
