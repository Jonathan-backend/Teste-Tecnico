import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { PessoasList } from "@/pages/PessoasList";

vi.mock("@/hooks/usePessoas", () => ({
  usePessoas: () => ({
    data: {
      items: [
        {
          id: "1",
          nome: "Maria de Teste",
          dataNascimento: new Date("1995-04-13T00:00:00Z"),
          idade: 30,
        },
      ],
      total: 1,
      page: 1,
      pageSize: 8,
    },
    isLoading: false,
  }),
  useDeletePessoa: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

describe("PessoasList", () => {
  it("deve renderizar listagem com dados vindos do hook", () => {
    render(
      <MemoryRouter>
        <PessoasList />
      </MemoryRouter>
    );

    expect(screen.getByRole("heading", { name: "Pessoas" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Adicionar Pessoa" })).toBeInTheDocument();
    expect(screen.getByText("Maria de Teste")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Editar 1/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Deletar 1/i })).toBeInTheDocument();
  });
});
