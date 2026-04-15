import { expect, test } from "@playwright/test";

test("deve navegar entre dashboard, pessoas e relatorios", async ({ page }) => {
  await page.goto("/");
  const sidebar = page.getByRole("complementary");

  await expect(page.getByText("Minhas Finanças")).toBeVisible();
  await expect(page.getByRole("heading", { name: "Bem-vindo!" })).toBeVisible();

  await sidebar.getByRole("link", { name: "Pessoas" }).click();
  await expect(page.getByRole("heading", { name: "Pessoas" })).toBeVisible();

  await sidebar.getByRole("link", { name: "Relatórios" }).click();
  await expect(page.getByRole("heading", { name: "Totais por Pessoa" })).toBeVisible();
  await expect(page.getByRole("table", { name: "Tabela de dados" })).toBeVisible();
});
