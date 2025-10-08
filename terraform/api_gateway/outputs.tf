output "invoke_url" {
  description = "URL de invocação da API Gateway"
  value       = aws_apigatewayv2_stage.default_stage.invoke_url
}
