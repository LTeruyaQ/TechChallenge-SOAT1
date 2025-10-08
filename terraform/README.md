# Terraform para MecanicaOS na AWS EKS

Este diretório contém a configuração do Terraform para implantar a aplicação MecanicaOS no Amazon EKS (Elastic Kubernetes Service) e expô-la através de um API Gateway.

## Pré-requisitos

Antes de começar, garanta que você tenha as seguintes ferramentas instaladas e configuradas:

- [Terraform](https://learn.hashicorp.com/tutorials/terraform/install-cli) (versão >= 1.5.0)
- [AWS CLI](https://aws.amazon.com/cli/) (configurado com suas credenciais da AWS)
- [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/) (para interagir com o cluster, se necessário)

## Configuração

Você precisa fornecer valores para algumas variáveis antes de executar a configuração. A maneira recomendada é criar um arquivo chamado `terraform.tfvars` dentro deste diretório.

1.  **Imagem Docker:**
    Altere o valor da variável `docker_image`. Certifique-se de que a imagem esteja publicada em um registro de contêiner acessível (como Docker Hub ou Amazon ECR).

2.  **Credenciais do Supabase:**
    Forneça a URL e a chave (key) do seu projeto Supabase.

**Exemplo de arquivo `terraform.tfvars`:**

```hcl
# terraform.tfvars

docker_image = "seu-usuario-docker/mecanicaos-api:latest"
supabase_url = "https://<id-do-seu-projeto>.supabase.co"
supabase_key = "<sua-chave-anon-supabase>"
```

> **⚠️ Aviso de Segurança:** A variável `supabase_key` está marcada como `sensitive`, mas ainda será armazenada em texto plano no arquivo de estado do Terraform (`terraform.tfstate`). Para ambientes de produção, é altamente recomendável usar um [backend remoto seguro](https://www.terraform.io/language/settings/backends/s3) para proteger seus segredos.

## Como Implantar

1.  **Inicializar o Terraform:**
    Navegue até este diretório (`terraform/`) e execute:
    ```sh
    terraform init
    ```

2.  **Planejar a Implantação:**
    Revise os recursos que o Terraform criará:
    ```sh
    terraform plan
    ```

3.  **Aplicar a Configuração:**
    Implante a infraestrutura na AWS:
    ```sh
    terraform apply
    ```
    Você será solicitado a confirmar a ação. Digite `yes` para prosseguir.

## Saídas (Outputs)

Após a implantação bem-sucedida, o Terraform exibirá a URL pública do seu API Gateway:

-   `api_gateway_url`: O endpoint que você pode usar para acessar sua aplicação.

## Como Destruir

Para remover todos os recursos criados por esta configuração, execute:

```sh
terraform destroy
```
