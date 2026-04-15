import { formatCurrency, formatDateForInput } from "@/lib/formatters";

describe("formatters", () => {
  it("deve formatar moeda em pt-BR", () => {
    expect(formatCurrency(1234.5)).toContain("1.234,50");
  });

  it("deve formatar data para input", () => {
    const date = new Date(2026, 3, 13);

    expect(formatDateForInput(date)).toBe("2026-04-13");
  });
});
