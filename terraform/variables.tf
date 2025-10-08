variable "aws_region" {
  description = "Região da AWS"
  type        = string
  default     = "us-east-1"
}

variable "cluster_name" {
  description = "Nome do cluster EKS"
  type        = string
  default     = "mecanicaos-demo"
}

variable "docker_image" {
  description = "Imagem Docker do MecanicaOS API"
  type        = string
  default     = "your-dockerhub-user/mecanicaos-api:latest"
}

variable "supabase_url" {
  description = "URL do Supabase (banco de dados externo)"
  type        = string
}

variable "supabase_key" {
  description = "Chave do Supabase (API key)"
  type        = string
  sensitive   = true
}
