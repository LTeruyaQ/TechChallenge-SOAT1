variable "cluster_name" {
  description = "Nome do cluster EKS"
  type        = string
}

variable "node_count" {
  description = "Número de nós no node group"
  type        = number
  default     = 1
}

variable "docker_image" {
  description = "Imagem Docker do MecanicaOS API"
  type        = string
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
