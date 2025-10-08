output "api_gateway_url" {
  description = "Endpoint público da API"
  value       = module.api_gateway.invoke_url
}
