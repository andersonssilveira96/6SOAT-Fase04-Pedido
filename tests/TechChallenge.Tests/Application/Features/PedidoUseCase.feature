Feature: PedidoUseCase
  Para gerenciar pedidos no sistema
  Como um desenvolvedor
  Quero validar os cenários principais da classe PedidoUseCase
  
Scenario: Inserir um pedido com produtos válidos
  Given os produtos com IDs 1 e 2 existem no repositório
  When um pedido com esses produtos é cadastrado
  Then o pedido deve ser salvo com sucesso
  And uma mensagem deve ser enviada ao message broker

Scenario: Atualizar o status de um pedido com sucesso
  Given um pedido com ID 1 existe no repositório com status "PagamentoPendente"
  When o status do pedido é atualizado para "Recebido"
  Then o status do pedido deve ser "Recebido"

Scenario: Consultar a lista de pedidos pendentes e não finalizados
  Given pedidos com diferentes status existem no repositório
  When a lista de pedidos é consultada
  Then somente pedidos não finalizados devem ser retornados
  And os pedidos devem ser ordenados por status e data de criação

Scenario: Consultar o status de pagamento de um pedido
  Given um pedido com ID 1 existe no repositório com status "Recebido"
  When o status de pagamento é consultado
  Then o pagamento deve estar aprovado
