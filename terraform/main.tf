terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "> 5.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "> 2.29"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

# Cluster EKS
module "eks" {
  source       = "./eks"
  cluster_name = var.cluster_name
  node_count   = 1
  docker_image = var.docker_image
  supabase_url = var.supabase_url
  supabase_key = var.supabase_key
}

# Provedor Kubernetes (depois do cluster pronto)
provider "kubernetes" {
  host                   = module.eks.cluster_endpoint
  cluster_ca_certificate = base64decode(module.eks.cluster_certificate_authority_data)
  token                  = module.eks.cluster_token
}

# API Gateway para expor a API
module "api_gateway" {
  source          = "./api_gateway"
  eks_service_url = module.eks.service_url
}
