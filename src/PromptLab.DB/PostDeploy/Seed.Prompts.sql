/*
  Prompts sinteticos para volumen / performance (57 filas, idempotente por titulo).
*/

CREATE TABLE [#SeedPrompts]
(
    [Title] NVARCHAR(200) COLLATE DATABASE_DEFAULT NOT NULL,
    [Description] NVARCHAR(1000) COLLATE DATABASE_DEFAULT NULL,
    [Content] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT NOT NULL,
    [Category] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [Language] NVARCHAR(20) COLLATE DATABASE_DEFAULT NULL,
    [ModelHint] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [TargetModelId] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [Temperature] DECIMAL(4, 2) NULL,
    [MaxTokens] INT NULL,
    [TopP] DECIMAL(4, 2) NULL
);

INSERT INTO [#SeedPrompts]
(
    [Title],
    [Description],
    [Content],
    [Category],
    [Language],
    [ModelHint],
    [TargetModelId],
    [Temperature],
    [MaxTokens],
    [TopP]
)
VALUES
(N'Propuesta de valor B2B para reunion de descubrimiento', N'Genera un pitch ejecutivo alineado a dolor y resultado.', N'Cliente: {{cliente}}. Industria: {{industria}}. Producto: {{producto}}. Objetivo: {{objetivo}}. Escribe propuesta de valor en 5 bullets y cierre con CTA.', N'Sales', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.65, 420, 0.95),
(N'Email de prospeccion en frio para CTO', N'Email corto y tecnico para agendar reunion.', N'Destinatario: {{nombre}}. Empresa: {{empresa}}. Stack: {{stack}}. Valor: {{valor}}. Redacta email frio max 120 palabras.', N'Sales', N'es', N'gpt-4o', N'gpt-5.5', 0.55, 380, 0.92),
(N'Script de llamada de descubrimiento SaaS', N'Guion con preguntas abiertas y manejo de objeciones.', N'Solucion: {{solucion}}. ICP: {{icp}}. Duracion minutos: {{minutos}}. Genera script con apertura, discovery y cierre.', N'Sales', N'es', N'gpt-4o-mini', N'gpt-4o', 0.60, 500, 0.90),
(N'Secuencia de nurturing post-demo', N'Tres correos espaciados para avanzar evaluacion.', N'Prospecto: {{prospecto}}. Interes: {{interes}}. Riesgo: {{riesgo}}. Escribe secuencia de 3 emails con asuntos y cuerpos.', N'Sales', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.70, 650, 0.95),
(N'Email de recuperacion de oportunidad stalled', N'Reactivar conversacion sin presionar.', N'Contacto: {{contacto}}. Ultimo tema: {{tema}}. Fecha ultimo touch: {{fecha}}. Redacta email de reactivacion empatico.', N'Sales', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.50, 360, 0.93),
(N'Resumen de conversacion comercial para CRM', N'Notas estructuradas para Salesforce/HubSpot.', N'Transcripcion: {{transcripcion}}. Etapa: {{etapa}}. Genera resumen: pain, budget, authority, timeline, next steps.', N'Sales', N'es', N'gpt-4o', N'gpt-5.5', 0.35, 520, 0.88),
(N'Argumentario frente a competidor', N'Comparativa respetuosa y diferenciadores.', N'Competidor: {{competidor}}. Fortalezas propias: {{fortalezas}}. Debilidades conocidas competidor: {{debilidades}}. Crea argumentario con FAQs.', N'Sales', N'en', N'gpt-4o', N'gpt-5.5', 0.45, 600, 0.90),
(N'One-pager ejecutivo para sponsor', N'Documento breve para aprobacion interna del cliente.', N'Proyecto: {{proyecto}}. ROI esperado: {{roi}}. Riesgos: {{riesgos}}. Estructura one-pager con secciones claras.', N'Sales', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.40, 700, 0.88),

(N'Copy Google Ads RSA para SaaS', N'Headlines y descripciones dentro de limites de caracteres.', N'Producto: {{producto}}. Keywords: {{keywords}}. Propuesta: {{propuesta}}. Genera 15 headlines y 4 descripciones.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.75, 480, 0.95),
(N'Post de LinkedIn para lanzamiento de feature', N'Tono profesional con storytelling.', N'Feature: {{feature}}. Beneficio: {{beneficio}}. Metrica: {{metrica}}. Audiencia: {{audiencia}}. Redacta post con hook y CTA.', N'Marketing', N'es', N'gpt-4o', N'gpt-5.5', 0.70, 400, 0.93),
(N'Newsletter trimestral para clientes', N'Actualizaciones de producto y mejores practicas.', N'Trimestre: {{trimestre}}. Highlights: {{highlights}}. Enlaces: {{enlaces}}. Escribe newsletter HTML-friendly en secciones.', N'Marketing', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.55, 900, 0.90),
(N'Hero section landing page producto analitico', N'Copy orientado a conversion B2B.', N'Producto: {{producto}}. Persona: {{persona}}. Prueba social: {{prueba_social}}. Genera headline, subheadline y 3 bullets.', N'Marketing', N'en', N'gpt-4o', N'gpt-5.5', 0.68, 450, 0.92),
(N'Brief creativo para campaña de demand gen', N'Objetivos, mensajes y canales.', N'Campaña: {{campana}}. Presupuesto: {{presupuesto}}. KPI: {{kpi}}. Crea brief con insight, idea creativa y plan de medios.', N'Marketing', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.60, 750, 0.88),
(N'Descripcion App Store iOS', N'Texto ASO con keywords naturales.', N'App: {{app}}. Categoria: {{categoria}}. Diferenciador: {{diferenciador}}. Escribe titulo, subtitulo y descripcion.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-4o', 0.72, 500, 0.94),
(N'Meta descripcion SEO cluster temático', N'Meta tags para grupo de URLs.', N'Tema: {{tema}}. Intencion: {{intencion}}. URL canonica: {{url}}. Genera title y meta description <= 160 caracteres.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.50, 320, 0.90),
(N'Guion video producto 90 segundos', N'Voiceover con tiempos aproximados.', N'Producto: {{producto}}. Publico: {{publico}}. CTA: {{cta}}. Escribe guion con marcas de tiempo cada 10s.', N'Marketing', N'es', N'gpt-4o', N'gpt-5.5', 0.65, 600, 0.91),
(N'Caso de éxito formato storytelling', N'Cliente anonimizado con metricas.', N'Cliente: {{cliente}}. Desafio: {{desafio}}. Solucion: {{solucion}}. Resultado: {{resultado}}. Redacta caso 800 palabras.', N'Marketing', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.58, 1200, 0.89),
(N'Encuesta NPS follow-up texto', N'Mensaje tras NPS bajo o alto.', N'Puntuacion: {{puntuacion}}. Comentario: {{comentario}}. Marca: {{marca}}. Genera email de seguimiento personalizado.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-4o', 0.62, 350, 0.93),

(N'Code review checklist comentarios PR', N'Comentarios constructivos por archivo.', N'Lenguaje: {{lenguaje}}. Cambio: {{cambio}}. Riesgos: {{riesgos}}. Lista comentarios priorizados severidad S1-S4.', N'Engineering', N'en', N'gpt-4o', N'gpt-5.5', 0.35, 700, 0.88),
(N'RFC microservicios trafico alto', N'Documento tecnico con alternativas.', N'Dominio: {{dominio}}. SLA: {{sla}}. Limitaciones: {{limitaciones}}. Escribe RFC con contexto, opciones y decision.', N'Engineering', N'es', N'claude-3-5-sonnet', N'claude-opus', 0.30, 1500, 0.85),
(N'Documentacion OpenAPI endpoint REST', N'Especificacion YAML/JSON breve.', N'Metodo: {{metodo}}. Ruta: {{ruta}}. Errores: {{errores}}. Genera descripcion OpenAPI-style en markdown.', N'Engineering', N'en', N'gpt-4o', N'gpt-5.5', 0.28, 800, 0.87),
(N'Descripcion de PR para changelog', N'Resumen para usuarios y notas internas.', N'Commits: {{commits}}. Impacto: {{impacto}}. Rollback: {{rollback}}. Redacta descripcion PR y bullets changelog.', N'Engineering', N'es', N'gpt-4o-mini', N'gpt-4o', 0.40, 550, 0.90),
(N'Generacion de tests unitarios C#', N'Casos felices y borde con xUnit.', N'Clase: {{clase}}. Metodo: {{metodo}}. Dependencias: {{dependencias}}. Genera tests con Arrange-Act-Assert.', N'Engineering', N'en', N'gpt-4o', N'gpt-5.5', 0.32, 900, 0.86),
(N'Troubleshooting playbook latencia API', N'Pasos de diagnostico ordenados.', N'Sintoma: {{sintoma}}. Ambiente: {{ambiente}}. Metricas: {{metricas}}. Crea playbook con checks y comandos sugeridos.', N'Engineering', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.33, 850, 0.88),
(N'Mensaje de breaking change para clientes API', N'Comunicacion clara con migracion.', N'Version: {{version}}. Cambio: {{cambio}}. Fecha: {{fecha}}. Redacta email y tabla de migracion.', N'Engineering', N'en', N'gpt-4o', N'gpt-5.5', 0.38, 650, 0.89),
(N'Checklist de release canary', N'Lista verificable pre y post deploy.', N'Servicio: {{servicio}}. Region: {{region}}. Umbrales: {{umbrales}}. Genera checklist con owners.', N'Engineering', N'es', N'gpt-4o-mini', N'gpt-4o', 0.36, 520, 0.91),
(N'Especificacion tecnica de feature flag', N'Criterios de activacion y rollback.', N'Flag: {{flag}}. Usuarios: {{usuarios}}. Metricas: {{metricas}}. Escribe spec con estados y pruebas.', N'Engineering', N'en', N'claude-3-5-sonnet', N'claude-sonnet', 0.34, 720, 0.87),

(N'Respuesta empatica a reclamo de facturacion', N'Tono calmado y accion concreta.', N'Cliente: {{cliente}}. Monto: {{monto}}. Detalle: {{detalle}}. Politica: {{politica}}. Redacta respuesta con pasos siguientes.', N'Support', N'es', N'gpt-4o-mini', N'gpt-4o', 0.45, 420, 0.92),
(N'Escalacion a ingenieria con contexto', N'Ticket estructurado para dev.', N'Ticket: {{ticket}}. Pasos reproducir: {{pasos}}. Logs: {{logs}}. Severidad: {{severidad}}. Genera resumen escalacion.', N'Support', N'en', N'gpt-4o', N'gpt-5.5', 0.40, 600, 0.88),
(N'Articulo base de conocimiento error 503', N'Articulo self-service.', N'Servicio: {{servicio}}. Causas comunes: {{causas}}. Workaround: {{workaround}}. Escribe KB con TOC y FAQ.', N'Support', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.42, 900, 0.90),
(N'Follow-up CSAT baja puntuacion', N'Email para recuperar confianza.', N'Encuesta: {{encuesta}}. Comentario: {{comentario}}. Owner: {{owner}}. Redacta follow-up con oferta de llamada.', N'Support', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.48, 380, 0.91),
(N'Macro de respuesta incidente general', N'Plantilla con placeholders legales.', N'Incidente: {{incidente}}. Estado: {{estado}}. ETA: {{eta}}. Genera macro multilenguaje ES/EN.', N'Support', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.52, 450, 0.93),
(N'Email de cierre satisfactorio de ticket', N'Cierre con resumen y encuesta.', N'Ticket: {{ticket}}. Solucion: {{solucion}}. Tiempo: {{tiempo}}. Escribe email cierre y link encuesta.', N'Support', N'es', N'gpt-4o', N'gpt-4o', 0.55, 340, 0.92),
(N'Comunicacion mantenimiento programado P1', N'Aviso proactivo a clientes enterprise.', N'Ventana: {{ventana}}. Impacto: {{impacto}}. Mitigaciones: {{mitigaciones}}. Redacta comunicado formal.', N'Support', N'en', N'gpt-4o', N'gpt-5.5', 0.43, 500, 0.89),
(N'Plantilla status page incidente resolved', N'Actualizacion publica post-mortem light.', N'Servicio: {{servicio}}. Inicio: {{inicio}}. Fin: {{fin}}. Causa raiz alta nivel: {{causa}}. Escribe update status page.', N'Support', N'es', N'gpt-4o-mini', N'gpt-4o', 0.46, 400, 0.90),

(N'Runbook deploy blue-green', N'Pasos operativos con verificaciones.', N'Sistema: {{sistema}}. Version: {{version}}. Checks: {{checks}}. Genera runbook numerado con rollback.', N'Operations', N'es', N'gpt-4o', N'gpt-5.5', 0.38, 950, 0.87),
(N'Solicitud de cambio CAB', N'Formulario estandar de cambio.', N'Cambio: {{cambio}}. Riesgo: {{riesgo}}. Ventana: {{ventana}}. Stakeholders: {{stakeholders}}. Completa plantilla CAB.', N'Operations', N'en', N'claude-3-5-sonnet', N'claude-sonnet', 0.36, 800, 0.86),
(N'Postmortem timeline incidente', N'Linea de tiempo y acciones correctivas.', N'ID: {{id}}. Impacto: {{impacto}}. Eventos: {{eventos}}. Estructura postmortem blameless.', N'Operations', N'es', N'gpt-4o', N'gpt-5.5', 0.34, 1100, 0.85),
(N'Comunicado interno fin de incidente', N'Mensaje a toda la empresa.', N'Incidente: {{incidente}}. Duracion: {{duracion}}. Lecciones: {{lecciones}}. Redacta comunicado ejecutivo interno.', N'Operations', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.40, 650, 0.88),
(N'Checklist rollback base de datos', N'Pasos seguros con backups.', N'BD: {{bd}}. Cambio: {{cambio}}. Backup: {{backup}}. Lista checklist con orden estricto.', N'Operations', N'en', N'gpt-4o-mini', N'gpt-4o', 0.37, 700, 0.89),
(N'Handover turno NOC', N'Formato de traspaso entre turnos.', N'Alertas abiertas: {{alertas}}. Cambios: {{cambios}}. Riesgos: {{riesgos}}. Genera handover conciso.', N'Operations', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.44, 480, 0.91),

(N'Job description senior backend engineer', N'JD inclusiva con requisitos claros.', N'Empresa: {{empresa}}. Stack: {{stack}}. Ubicacion: {{ubicacion}}. Escribe JD con responsabilidades y seniority.', N'HR', N'en', N'gpt-4o', N'gpt-5.5', 0.50, 900, 0.90),
(N'Feedback performance construcción STAR', N'Comentarios accionables trimestrales.', N'Empleado: {{empleado}}. Logros: {{logros}}. Areas mejora: {{areas}}. Genera feedback formato STAR.', N'HR', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.48, 700, 0.89),
(N'Email onboarding día 1 remoto', N'Bienvenida y checklist primer dia.', N'Nombre: {{nombre}}. Rol: {{rol}}. Herramientas: {{herramientas}}. Redacta email onboarding amigable.', N'HR', N'es', N'gpt-4o-mini', N'gpt-4o', 0.60, 520, 0.92),
(N'Borrador carta oferta laboral', N'Plantilla legal-light para revision abogado.', N'Candidato: {{candidato}}. Rol: {{rol}}. Compensacion: {{compensacion}}. Genera borrador oferta con placeholders legales.', N'HR', N'es', N'gpt-4o', N'gpt-5.5', 0.35, 850, 0.86),
(N'Guia de entrevista comportamental', N'Preguntas y rúbrica de evaluación.', N'Competencia: {{competencia}}. Nivel: {{nivel}}. Situaciones: {{situaciones}}. Crea guia con preguntas STAR.', N'HR', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.42, 750, 0.88),

(N'User story formato INVEST', N'Historia lista para backlog.', N'Actor: {{actor}}. Necesidad: {{necesidad}}. Beneficio: {{beneficio}}. Criterios aceptacion: {{criterios}}. Escribe user story.', N'Product', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.55, 480, 0.91),
(N'Seccion PRD riesgos y dependencias', N'Bloque estandar de PRD.', N'Feature: {{feature}}. Equipos: {{equipos}}. Dependencias: {{dependencias}}. Riesgos: {{riesgos}}. Redacta seccion PRD.', N'Product', N'en', N'gpt-4o', N'gpt-5.5', 0.45, 820, 0.88),
(N'Justificacion roadmap trimestral', N'Narrativa para stakeholders.', N'Objetivos: {{objetivos}}. Métricas: {{metricas}}. Apuestas: {{apuestas}}. Escribe justificacion roadmap Q.', N'Product', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.52, 900, 0.90),
(N'Hipotesis para test A/B onboarding', N'Hipotesis medible y diseno experimental.', N'Pantalla: {{pantalla}}. Metrica: {{metrica}}. Tráfico esperado: {{trafico}}. Formula hipotesis y variantes.', N'Product', N'es', N'gpt-4o', N'gpt-4o', 0.58, 600, 0.92),
(N'Release notes usuario final', N'Notas claras sin jargon interno.', N'Version: {{version}}. Cambios: {{cambios}}. Impacto usuario: {{impacto}}. Genera release notes amigables.', N'Product', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.62, 550, 0.93),

(N'Interpretacion caida de conversion semanal', N'Analisis cualitativo de metricas.', N'Embudo: {{embudo}}. Caida pct: {{caida}}. Segmentos: {{segmentos}}. Hipotesis y siguientes pasos.', N'Data', N'es', N'gpt-4o', N'gpt-5.5', 0.40, 700, 0.88),
(N'Documentacion SQL query agregados', N'Comentarios y ejemplo de uso.', N'Tabla: {{tabla}}. Agregacion: {{agregacion}}. Filtros: {{filtros}}. Documenta query SQL con ejemplo salida.', N'Data', N'en', N'claude-3-5-sonnet', N'claude-sonnet', 0.32, 900, 0.86),
(N'Descripcion de dashboard ejecutivo KPIs', N'Explicacion de paneles para C-level.', N'KPIs: {{kpis}}. Fuente datos: {{fuente}}. Cadencia: {{cadencia}}. Redacta descripcion ejecutiva del dashboard.', N'Data', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.38, 650, 0.87),
(N'Explicacion anomalia metricas fin de semana', N'Narrativa de datos con cautela estadistica.', N'Metrica: {{metrica}}. Patron: {{patron}}. Contexto: {{contexto}}. Explica posibles causas y validaciones.', N'Data', N'es', N'gpt-4o-mini', N'gpt-4o', 0.42, 600, 0.90),
(N'Definicion de cohortes para retencion', N'Definiciones reproducibles.', N'Producto: {{producto}}. Evento activacion: {{evento}}. Ventana: {{ventana}}. Define cohortes y queries sugeridas.', N'Data', N'en', N'gpt-4o', N'gpt-5.5', 0.36, 780, 0.87),
(N'Resumen exploratorio dataset de ventas', N'EDA narrativo para no tecnicos.', N'Columnas: {{columnas}}. Periodo: {{periodo}}. Objetivo: {{objetivo}}. Escribe resumen EDA con insights y riesgos.', N'Data', N'es', N'gpt-4o', N'gpt-5.5', 0.44, 850, 0.89);

DECLARE @Title NVARCHAR(200);
DECLARE @Description NVARCHAR(1000);
DECLARE @Content NVARCHAR(MAX);
DECLARE @Category NVARCHAR(100);
DECLARE @Language NVARCHAR(20);
DECLARE @ModelHint NVARCHAR(100);
DECLARE @TargetModelId NVARCHAR(100);
DECLARE @Temperature DECIMAL(4, 2);
DECLARE @MaxTokens INT;
DECLARE @TopP DECIMAL(4, 2);

DECLARE seed_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    [Title],
    [Description],
    [Content],
    [Category],
    [Language],
    [ModelHint],
    [TargetModelId],
    [Temperature],
    [MaxTokens],
    [TopP]
FROM [#SeedPrompts];

OPEN seed_cursor;

FETCH NEXT FROM seed_cursor
INTO
    @Title,
    @Description,
    @Content,
    @Category,
    @Language,
    @ModelHint,
    @TargetModelId,
    @Temperature,
    @MaxTokens,
    @TopP;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = @Title)
    BEGIN
        EXEC [dbo].[Prompt_Create]
            @Title = @Title,
            @Description = @Description,
            @Content = @Content,
            @Category = @Category,
            @Language = @Language,
            @ModelHint = @ModelHint,
            @TargetModelId = @TargetModelId,
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP;
    END;

    FETCH NEXT FROM seed_cursor
    INTO
        @Title,
        @Description,
        @Content,
        @Category,
        @Language,
        @ModelHint,
        @TargetModelId,
        @Temperature,
        @MaxTokens,
        @TopP;
END;

CLOSE seed_cursor;
DEALLOCATE seed_cursor;

DROP TABLE [#SeedPrompts];
