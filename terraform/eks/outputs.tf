data "aws_eks_cluster_auth" "this" {
  name = aws_eks_cluster.demo.name
}

output "cluster_endpoint" {
  description = "Endpoint for the EKS cluster"
  value       = aws_eks_cluster.demo.endpoint
}

output "cluster_certificate_authority_data" {
  description = "Certificate authority data for the EKS cluster"
  value       = aws_eks_cluster.demo.certificate_authority[0].data
}

output "cluster_token" {
  description = "Token to authenticate with the EKS cluster"
  value       = data.aws_eks_cluster_auth.this.token
  sensitive   = true
}

output "service_url" {
  description = "URL do serviço Kubernetes LoadBalancer"
  value       = kubernetes_service.mecanicaos_service.status[0].load_balancer[0].ingress[0].hostname
}
