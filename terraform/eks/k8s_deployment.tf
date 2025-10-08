resource "kubernetes_secret" "supabase" {
  metadata {
    name = "supabase-secret"
  }

  data = {
    SUPABASE_URL = var.supabase_url
    SUPABASE_KEY = var.supabase_key
  }
}

resource "kubernetes_deployment" "mecanicaos_api" {
  metadata {
    name = "mecanicaos-api"
    labels = {
      app = "mecanicaos-api"
    }
  }

  spec {
    replicas = 1
    selector {
      match_labels = {
        app = "mecanicaos-api"
      }
    }

    template {
      metadata {
        labels = {
          app = "mecanicaos-api"
        }
      }

      spec {
        container {
          name  = "mecanicaos-api"
          image = var.docker_image
          port {
            container_port = 8080
          }

          env_from {
            secret_ref {
              name = kubernetes_secret.supabase.metadata[0].name
            }
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "mecanicaos_service" {
  metadata {
    name = "mecanicaos-service"
  }

  spec {
    selector = {
      app = "mecanicaos-api"
    }

    port {
      port        = 80
      target_port = 8080
    }

    type = "LoadBalancer"
  }
}

output "service_url" {
  value = kubernetes_service.mecanicaos_service.status[0].load_balancer[0].ingress[0].hostname
}
