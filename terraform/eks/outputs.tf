output "cluster_endpoint" {
  description = "Endpoint for EKS cluster"
  value       = aws_eks_cluster.demo.endpoint
}

output "cluster_certificate_authority_data" {
  description = "Certificate authority data for EKS cluster"
  value       = aws_eks_cluster.demo.certificate_authority[0].data
}

output "cluster_token" {
  description = "Token for EKS cluster authentication"
  value       = data.aws_eks_cluster_auth.demo.token
}

output "service_hostname" {
  description = "Hostname do serviço Kubernetes"
  value       = kubernetes_service.mecanicaos_service.status[0].load_balancer[0].ingress[0].hostname
}
