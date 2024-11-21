Feature: ProdutosUseCase

  # Cadastrar Produto
  Scenario: Cadastrar um produto com categoria v�lida
    Given a categoria "Lanche" existe
    When o produto "BigMc" com valor "15" � cadastrado na categoria "Lanche"
    Then o produto deve ser cadastrado com sucesso
    And o produto deve ter a categoria "Lanche"

  Scenario: Tentar cadastrar um produto com categoria inv�lida
    Given a categoria "Eletr�nicos" n�o existe
    When o produto "BigMc" com valor "15" � cadastrado com uma categoria inv�lida
    Then uma exce��o de categoria inv�lida deve ser lan�ada


  # Atualizar Produto
  Scenario: Atualizar um produto com categoria v�lida
    Given o produto "BigMc" existe com valor "15"   
    And a categoria "Lanche" existe
    And a categoria "Bebida" existe
    When o produto "BigMc" � atualizado para o valor "20" e categoria "Bebida"
    Then o produto deve ser atualizado com sucesso
    And o produto deve ter o valor "20" e a categoria "Bebida"

  Scenario: Tentar atualizar produto com categoria inv�lida
    Given o produto "BigMc" existe com valor "15"
    And a categoria "Eletr�nicos" n�o existe
    When o produto "BigMc" � atualizado para uma categoria inv�lida
    Then uma exce��o de categoria inv�lida deve ser lan�ada


  # Excluir Produto
  Scenario: Excluir um produto existente
    Given o produto "BigMc" existe
    When o produto "BigMc" � exclu�do
    Then o produto deve ser exclu�do com sucesso

  Scenario: Tentar excluir um produto inexistente
    Given o produto "BigMc2" n�o existe
    When o produto "BigMc2" � exclu�do
    Then uma exce��o de produto inv�lido deve ser lan�ada


  # Listar Produtos
  Scenario: Listar todos os produtos
    Given existem produtos cadastrados
    When a lista de produtos � consultada
    Then a lista de produtos deve ser retornada
    And a lista de produtos deve conter os produtos cadastrados


  # Obter Produto por ID
  Scenario: Obter produto por ID v�lido
    Given o produto "BigMc" existe
    When o produto � consultado pelo ID "1"
    Then o produto com ID "1" deve ser retornado

  Scenario: Tentar obter produto por ID inv�lido
    Given o produto "BigMc2" n�o existe
    When o produto � consultado pelo ID "999"
    Then uma exce��o de produto inv�lido deve ser lan�ada
