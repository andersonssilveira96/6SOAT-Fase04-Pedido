Feature: ProdutosUseCase

  # Cadastrar Produto
  Scenario: Cadastrar um produto com categoria válida
    Given a categoria "Lanche" existe
    When o produto "BigMc" com valor "15" é cadastrado na categoria "Lanche"
    Then o produto deve ser cadastrado com sucesso
    And o produto deve ter a categoria "Lanche"

  Scenario: Tentar cadastrar um produto com categoria inválida
    Given a categoria "Eletrônicos" não existe
    When o produto "BigMc" com valor "15" é cadastrado com uma categoria inválida
    Then uma exceção de categoria inválida deve ser lançada


  # Atualizar Produto
  Scenario: Atualizar um produto com categoria válida
    Given o produto "BigMc" existe com valor "15"   
    And a categoria "Lanche" existe
    And a categoria "Bebida" existe
    When o produto "BigMc" é atualizado para o valor "20" e categoria "Bebida"
    Then o produto deve ser atualizado com sucesso
    And o produto deve ter o valor "20" e a categoria "Bebida"

  Scenario: Tentar atualizar produto com categoria inválida
    Given o produto "BigMc" existe com valor "15"
    And a categoria "Eletrônicos" não existe
    When o produto "BigMc" é atualizado para uma categoria inválida
    Then uma exceção de categoria inválida deve ser lançada


  # Excluir Produto
  Scenario: Excluir um produto existente
    Given o produto "BigMc" existe
    When o produto "BigMc" é excluído
    Then o produto deve ser excluído com sucesso

  Scenario: Tentar excluir um produto inexistente
    Given o produto "BigMc2" não existe
    When o produto "BigMc2" é excluído
    Then uma exceção de produto inválido deve ser lançada


  # Listar Produtos
  Scenario: Listar todos os produtos
    Given existem produtos cadastrados
    When a lista de produtos é consultada
    Then a lista de produtos deve ser retornada
    And a lista de produtos deve conter os produtos cadastrados


  # Obter Produto por ID
  Scenario: Obter produto por ID válido
    Given o produto "BigMc" existe
    When o produto é consultado pelo ID "1"
    Then o produto com ID "1" deve ser retornado

  Scenario: Tentar obter produto por ID inválido
    Given o produto "BigMc2" não existe
    When o produto é consultado pelo ID "999"
    Then uma exceção de produto inválido deve ser lançada
