resource "aws_apigatewayv2_api" "http_api" {
  name          = "mecanicaos-api"
  protocol_type = "HTTP"
}

resource "aws_apigatewayv2_integration" "eks_backend" {
  api_id             = aws_apigatewayv2_api.http_api.id
  integration_type   = "HTTP_PROXY"
  integration_uri    = "http://${var.eks_service_url}"
  integration_method = "ANY"
}

resource "aws_apigatewayv2_route" "proxy_route" {
  api_id    = aws_apigatewayv2_api.http_api.id
  route_key = "ANY /{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.eks_backend.id}"
}

resource "aws_apigatewayv2_stage" "default_stage" {
  api_id      = aws_apigatewayv2_api.http_api.id
  name        = "$default"
  auto_deploy = true
}
