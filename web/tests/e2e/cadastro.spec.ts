import { expect, test } from "@playwright/test";

const apiBaseURL = process.env.PLAYWRIGHT_API_URL ?? "http://localhost:5000/api/v1.0";

test("deve criar categoria e pessoa pela interface", async ({ page, request }) => {
  const suffix = Date.now().toString();
  const categoria = `Categoria E2E ${suffix}`;
  const pessoa = `Pessoa E2E ${suffix}`;

  // cria categoria
  await page.goto("/categorias");
  await page.getByRole("button", { name: "Adicionar Categoria" }).click();
  await expect(page.getByRole("heading", { name: "Adicionar Categoria" })).toBeVisible();
  await page.getByLabel("Descrição").fill(categoria);
  await page.getByLabel("Finalidade").selectOption("ambas");
  const categoriaResponsePromise = page.waitForResponse((response) =>
    response.request().method() === "POST" && response.url().includes("/categorias"),
  );
  await page.getByRole("button", { name: "Salvar" }).click();
  const categoriaResponse = await categoriaResponsePromise;
  expect(categoriaResponse.ok()).toBeTruthy();
  await expect(page.getByRole("heading", { name: "Categorias" })).toBeVisible();

  await expect.poll(async () => {
    const response = await request.get(`${apiBaseURL}/categorias`, {
      params: { page: "1", pageSize: "50", search: categoria },
    });
    const body = await response.json();
    return body.items?.some((item: { descricao?: string; Descricao?: string }) =>
      (item.descricao ?? item.Descricao) === categoria,
    );
  }).toBeTruthy();

  
  await page.goto("/pessoas");
  await page.getByRole("button", { name: "Adicionar Pessoa" }).click();
  await expect(page.getByRole("heading", { name: "Adicionar Pessoa" })).toBeVisible();
  await page.getByLabel("Nome").fill(pessoa);
  await page.getByLabel("Data de Nascimento").fill("1995-04-13");
  const pessoaResponsePromise = page.waitForResponse((response) =>
    response.request().method() === "POST" && response.url().includes("/pessoas"),
  );
  await page.getByRole("button", { name: "Salvar" }).click();
  const pessoaResponse = await pessoaResponsePromise;
  expect(pessoaResponse.ok()).toBeTruthy();
  await expect(page.getByRole("heading", { name: "Pessoas" })).toBeVisible();

  await expect.poll(async () => {
    const response = await request.get(`${apiBaseURL}/pessoas`, {
      params: { page: "1", pageSize: "50", search: pessoa },
    });
    const body = await response.json();
    return body.items?.some((item: { nome?: string; Nome?: string }) =>
      (item.nome ?? item.Nome) === pessoa,
    );
  }).toBeTruthy();
});
