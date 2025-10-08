variable "cluster_name" {
  description = "Nome do cluster EKS"
  type        = string
}

variable "node_count" {
  description = "Número de nós no node group"
  type        = number
}

variable "supabase_url" {
  description = "URL do Supabase"
  type        = string
}

variable "supabase_key" {
  description = "Chave do Supabase"
  type        = string
  sensitive   = true
}

variable "docker_image" {
  description = "Imagem Docker do MecanicaOS API"
  type        = string
}
