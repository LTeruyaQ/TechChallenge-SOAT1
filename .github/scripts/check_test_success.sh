#!/bin/bash

# Define o arquivo de log para a saída dos testes
TEST_OUTPUT_FILE="test_output.txt"

# Executa os testes na solução inteira e redireciona a saída para o arquivo de log
echo "Executando testes para a solução MecanicaOS.sln..."
dotnet test MecanicaOS.sln --logger "console;verbosity=normal" > $TEST_OUTPUT_FILE

# Exibe o conteúdo do log para depuração no GitHub Actions
echo "--- Log de Saída dos Testes ---"
cat $TEST_OUTPUT_FILE
echo "------------------------------"

# Extrai o número total de testes e o número de testes aprovados de suas respectivas linhas
total_tests=$(grep -oP 'Total tests: \K\d+' $TEST_OUTPUT_FILE)
passed_tests=$(grep -oP 'Passed: \K\d+' $TEST_OUTPUT_FILE)

# Verifica se os valores foram extraídos corretamente
if [ -z "$total_tests" ] || [ -z "$passed_tests" ]; then
  echo "Erro: Não foi possível extrair o número total ou de testes aprovados."
  echo "Total extraído: '$total_tests'"
  echo "Aprovados extraído: '$passed_tests'"
  echo "O build irá falhar."
  exit 1
fi

# Evita divisão por zero se o total de testes for 0
if [ "$total_tests" -eq 0 ]; then
  echo "Nenhum teste foi executado. O build pode prosseguir."
  exit 0
fi

# Calcula a porcentagem de sucesso
success_percentage=$(( (passed_tests * 100) / total_tests ))

echo "Total de testes: $total_tests"
echo "Testes aprovados: $passed_tests"
echo "Porcentagem de sucesso: $success_percentage%"

# Verifica se a porcentagem é de pelo menos 90%
if [ "$success_percentage" -ge 90 ]; then
  echo "A porcentagem de sucesso dos testes é de $success_percentage%, que é igual ou superior a 90%. O build pode prosseguir."
  exit 0
else
  echo "A porcentagem de sucesso dos testes é de $success_percentage%, que é inferior a 90%. O build irá falhar."
  exit 1
fi
