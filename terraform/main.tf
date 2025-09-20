# Arquivo Terraform de exemplo
# Adicione aqui a configuração da sua infraestrutura.

provider "aws" {
  region = "us-east-1" # Defina a região da sua AWS
}

resource "aws_instance" "example" {
  ami           = "ami-0c55b159cbfafe1f0" # Exemplo de AMI para Ubuntu
  instance_type = "t2.micro"

  tags = {
    Name = "Exemplo-Instancia-Terraform"
  }
}
