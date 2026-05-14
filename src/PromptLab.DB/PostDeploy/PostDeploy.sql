/*
  Seed basico opcional para desarrollo local.

  Los fragmentos de volumen (equivalentes a PostDeploy/Seed.*.sql) estan incluidos
  mas abajo separados por GO, para que el script generado funcione en SSMS sin
  modo SQLCMD y para evitar redeclaracion de variables en un mismo lote.
*/

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'general')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'General';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'marketing')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Marketing';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'coding')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Coding';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'sales')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Sales';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'support')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Support';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Email de seguimiento comercial')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Email de seguimiento comercial',
        @Description = N'Genera un email de seguimiento para prospectos con tono profesional.',
        @Content = N'Escribe un email de seguimiento para {{nombre_cliente}} sobre {{producto}} destacando beneficios y proximo paso.',
        @Category = N'Sales',
        @Language = N'es',
        @ModelHint = N'gpt-4o-mini',
        @TargetModelId = N'gpt-5.5',
        @Temperature = 0.70,
        @MaxTokens = 350,
        @TopP = 1.00;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Resumen tecnico de incidente')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Resumen tecnico de incidente',
        @Description = N'Construye un resumen tecnico estructurado para incidentes operativos.',
        @Content = N'Dado el incidente {{incidente_id}}, resume causa raiz, impacto, mitigacion y acciones de prevencion.',
        @Category = N'Operations',
        @Language = N'es',
        @ModelHint = N'claude-3-5-sonnet',
        @TargetModelId = N'claude-sonnet',
        @Temperature = 0.40,
        @MaxTokens = 500,
        @TopP = 0.95;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Asistente para documentacion de API')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Asistente para documentacion de API',
        @Description = N'Genera documentacion breve de endpoints para equipos internos.',
        @Content = N'Para el endpoint {{metodo}} {{ruta}}, describe objetivo, parametros, respuestas y ejemplos de error.',
        @Category = N'Engineering',
        @Language = N'en',
        @ModelHint = N'gemini-1.5-pro',
        @TargetModelId = N'gemini-2.5-pro',
        @Temperature = 0.30,
        @MaxTokens = 600,
        @TopP = 0.90;
END

DECLARE @PromptSales UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Email de seguimiento comercial'
);

DECLARE @PromptIncident UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Resumen tecnico de incidente'
);

DECLARE @PromptApiDoc UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Asistente para documentacion de API'
);

DECLARE @TagGeneral UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'general'
);

DECLARE @TagMarketing UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'marketing'
);

DECLARE @TagCoding UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'coding'
);

DECLARE @TagSales UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'sales'
);

DECLARE @TagSupport UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'support'
);

IF @PromptSales IS NOT NULL AND @TagSales IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptSales
         AND [TagId] = @TagSales
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptSales, @TagSales, SYSUTCDATETIME());
END

IF @PromptSales IS NOT NULL AND @TagMarketing IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptSales
         AND [TagId] = @TagMarketing
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptSales, @TagMarketing, SYSUTCDATETIME());
END

IF @PromptIncident IS NOT NULL AND @TagSupport IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptIncident
         AND [TagId] = @TagSupport
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptIncident, @TagSupport, SYSUTCDATETIME());
END

IF @PromptApiDoc IS NOT NULL AND @TagCoding IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptApiDoc
         AND [TagId] = @TagCoding
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptApiDoc, @TagCoding, SYSUTCDATETIME());
END

IF @PromptApiDoc IS NOT NULL AND @TagGeneral IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptApiDoc
         AND [TagId] = @TagGeneral
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptApiDoc, @TagGeneral, SYSUTCDATETIME());
END

IF @PromptSales IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[AnalysisRuns]
       WHERE [PromptId] = @PromptSales
         AND [Provider] = N'simulated'
   )
BEGIN
    EXEC [dbo].[Analyze_CreateRun]
        @PromptId = @PromptSales,
        @Provider = N'simulated',
        @ModelId = N'simulated-default',
        @Input = N'nombre_cliente=Acme Corp; producto=PromptLab Enterprise',
        @Output = N'[SIMULATED] Email generado para seguimiento comercial con CTA.',
        @Temperature = 0.70,
        @MaxTokens = 350,
        @TopP = 1.00,
        @PromptSnapshot = N'Escribe un email de seguimiento para {{nombre_cliente}} sobre {{producto}} destacando beneficios y proximo paso.',
        @PromptSnapshotHash = N'SEED-HASH',
        @Status = N'Completed',
        @ErrorMessage = NULL,
        @LatencyMs = 142;
END
GO

/* === Seed.Tags.sql === */
/*
  Tags adicionales para datos de volumen / performance (idempotente).
*/

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'hr')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'HR';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'legal')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Legal';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'finance')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Finance';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'product')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Product';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'data-analysis')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Data Analysis';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'content')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Content';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'operations')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Operations';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'devops')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'DevOps';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'security')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Security';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'testing')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Testing';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'onboarding')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Onboarding';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'reporting')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Reporting';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'customer-success')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Customer Success';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'ux-research')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'UX Research';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'compliance')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Compliance';
END;
GO

 /* === Seed.Prompts.sql === */
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
(N'Brief creativo para campaÃ±a de demand gen', N'Objetivos, mensajes y canales.', N'CampaÃ±a: {{campana}}. Presupuesto: {{presupuesto}}. KPI: {{kpi}}. Crea brief con insight, idea creativa y plan de medios.', N'Marketing', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.60, 750, 0.88),
(N'Descripcion App Store iOS', N'Texto ASO con keywords naturales.', N'App: {{app}}. Categoria: {{categoria}}. Diferenciador: {{diferenciador}}. Escribe titulo, subtitulo y descripcion.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-4o', 0.72, 500, 0.94),
(N'Meta descripcion SEO cluster temÃ¡tico', N'Meta tags para grupo de URLs.', N'Tema: {{tema}}. Intencion: {{intencion}}. URL canonica: {{url}}. Genera title y meta description <= 160 caracteres.', N'Marketing', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.50, 320, 0.90),
(N'Guion video producto 90 segundos', N'Voiceover con tiempos aproximados.', N'Producto: {{producto}}. Publico: {{publico}}. CTA: {{cta}}. Escribe guion con marcas de tiempo cada 10s.', N'Marketing', N'es', N'gpt-4o', N'gpt-5.5', 0.65, 600, 0.91),
(N'Caso de Ã©xito formato storytelling', N'Cliente anonimizado con metricas.', N'Cliente: {{cliente}}. Desafio: {{desafio}}. Solucion: {{solucion}}. Resultado: {{resultado}}. Redacta caso 800 palabras.', N'Marketing', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.58, 1200, 0.89),
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
(N'Feedback performance construcciÃ³n STAR', N'Comentarios accionables trimestrales.', N'Empleado: {{empleado}}. Logros: {{logros}}. Areas mejora: {{areas}}. Genera feedback formato STAR.', N'HR', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.48, 700, 0.89),
(N'Email onboarding dÃ­a 1 remoto', N'Bienvenida y checklist primer dia.', N'Nombre: {{nombre}}. Rol: {{rol}}. Herramientas: {{herramientas}}. Redacta email onboarding amigable.', N'HR', N'es', N'gpt-4o-mini', N'gpt-4o', 0.60, 520, 0.92),
(N'Borrador carta oferta laboral', N'Plantilla legal-light para revision abogado.', N'Candidato: {{candidato}}. Rol: {{rol}}. Compensacion: {{compensacion}}. Genera borrador oferta con placeholders legales.', N'HR', N'es', N'gpt-4o', N'gpt-5.5', 0.35, 850, 0.86),
(N'Guia de entrevista comportamental', N'Preguntas y rÃºbrica de evaluaciÃ³n.', N'Competencia: {{competencia}}. Nivel: {{nivel}}. Situaciones: {{situaciones}}. Crea guia con preguntas STAR.', N'HR', N'es', N'gemini-1.5-pro', N'gemini-2.5-pro', 0.42, 750, 0.88),

(N'User story formato INVEST', N'Historia lista para backlog.', N'Actor: {{actor}}. Necesidad: {{necesidad}}. Beneficio: {{beneficio}}. Criterios aceptacion: {{criterios}}. Escribe user story.', N'Product', N'es', N'gpt-4o-mini', N'gpt-5.5', 0.55, 480, 0.91),
(N'Seccion PRD riesgos y dependencias', N'Bloque estandar de PRD.', N'Feature: {{feature}}. Equipos: {{equipos}}. Dependencias: {{dependencias}}. Riesgos: {{riesgos}}. Redacta seccion PRD.', N'Product', N'en', N'gpt-4o', N'gpt-5.5', 0.45, 820, 0.88),
(N'Justificacion roadmap trimestral', N'Narrativa para stakeholders.', N'Objetivos: {{objetivos}}. MÃ©tricas: {{metricas}}. Apuestas: {{apuestas}}. Escribe justificacion roadmap Q.', N'Product', N'es', N'claude-3-5-sonnet', N'claude-sonnet', 0.52, 900, 0.90),
(N'Hipotesis para test A/B onboarding', N'Hipotesis medible y diseno experimental.', N'Pantalla: {{pantalla}}. Metrica: {{metrica}}. TrÃ¡fico esperado: {{trafico}}. Formula hipotesis y variantes.', N'Product', N'es', N'gpt-4o', N'gpt-4o', 0.58, 600, 0.92),
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
GO

 /* === Seed.PromptTags.sql === */
/*
  Relaciones PromptTags segun categoria (2-3 tags por prompt, idempotente).
*/

CREATE TABLE [#CategoryTagMap]
(
    [Category] NVARCHAR(100) COLLATE DATABASE_DEFAULT NOT NULL,
    [Slug] NVARCHAR(120) COLLATE DATABASE_DEFAULT NOT NULL
);

INSERT INTO [#CategoryTagMap] ([Category], [Slug])
VALUES
(N'Sales', N'sales'),
(N'Sales', N'marketing'),
(N'Sales', N'general'),
(N'Marketing', N'marketing'),
(N'Marketing', N'content'),
(N'Marketing', N'general'),
(N'Engineering', N'coding'),
(N'Engineering', N'devops'),
(N'Engineering', N'security'),
(N'Support', N'support'),
(N'Support', N'customer-success'),
(N'Support', N'general'),
(N'Operations', N'operations'),
(N'Operations', N'devops'),
(N'Operations', N'reporting'),
(N'HR', N'hr'),
(N'HR', N'onboarding'),
(N'HR', N'compliance'),
(N'Product', N'product'),
(N'Product', N'ux-research'),
(N'Product', N'general'),
(N'Data', N'data-analysis'),
(N'Data', N'reporting'),
(N'Data', N'coding');

INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
SELECT
    p.[Id],
    t.[Id],
    SYSUTCDATETIME()
FROM [dbo].[Prompts] AS p
INNER JOIN [#CategoryTagMap] AS m
    ON m.[Category] = p.[Category]
INNER JOIN [dbo].[Tags] AS t
    ON t.[Slug] = m.[Slug]
WHERE
    p.[IsActive] = 1
    AND NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[PromptTags] AS pt
        WHERE pt.[PromptId] = p.[Id]
          AND pt.[TagId] = t.[Id]
    );

DROP TABLE [#CategoryTagMap];
GO

 /* === Seed.TestSuites.sql === */
/*
  Test suites para ~75% de prompts activos (deterministico por Id), idempotente por nombre.
*/

DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @SuiteName NVARCHAR(200);
DECLARE @SuiteDescription NVARCHAR(1000) = N'Suite automatica de seed de volumen / performance.';

DECLARE suite_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    p.[Id],
    N'Suite seed v1: ' + LEFT(p.[Title], 180)
FROM [dbo].[Prompts] AS p
WHERE
    p.[IsActive] = 1
    AND ABS(CHECKSUM(CAST(p.[Id] AS VARBINARY(16)))) % 4 <> 0
    AND NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestSuites] AS ts
        WHERE ts.[PromptId] = p.[Id]
          AND ts.[Name] LIKE N'Suite seed v1:%'
    );

OPEN suite_cursor;

FETCH NEXT FROM suite_cursor
INTO @PromptId, @SuiteName;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC [dbo].[TestSuite_Create]
        @PromptId = @PromptId,
        @Name = @SuiteName,
        @Description = @SuiteDescription;

    FETCH NEXT FROM suite_cursor
    INTO @PromptId, @SuiteName;
END;

CLOSE suite_cursor;
DEALLOCATE suite_cursor;
GO

 /* === Seed.TestCases.sql === */
/*
  Cinco casos de prueba por suite seed (idempotente por SuiteId + nombre).
*/

DECLARE @SuiteId UNIQUEIDENTIFIER;

DECLARE case_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT [Id]
FROM [dbo].[TestSuites]
WHERE [Name] LIKE N'Suite seed v1:%'
  AND [IsActive] = 1;

OPEN case_cursor;

FETCH NEXT FROM case_cursor
INTO @SuiteId;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 01 - baseline'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 01 - baseline',
            @InputVariables = N'{"contexto":"Acme Corp","objetivo":"salida estable","restricciones":"max 200 tokens"}',
            @ExpectedOutput = N'[SEED] Salida esperada baseline sin caracteres especiales.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 02 - vacios parciales'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 02 - vacios parciales',
            @InputVariables = N'{"contexto":"","objetivo":"probar defaults","restricciones":"ninguna"}',
            @ExpectedOutput = N'[SEED] Debe manejar strings vacios sin error.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 03 - unicode y tildes'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 03 - unicode y tildes',
            @InputVariables = N'{"contexto":"Cliente con Ã± y Ã¡Ã©Ã­Ã³Ãº","objetivo":"UTF-8","restricciones":"emoji permitidos"}',
            @ExpectedOutput = N'[SEED] Respuesta con caracteres Unicode correctos.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 04 - valores largos'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 04 - valores largos',
            @InputVariables = N'{"contexto":"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris.","objetivo":"stress","restricciones":"sin truncar"}',
            @ExpectedOutput = N'[SEED] Debe procesar entradas largas sin degradar calidad.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 05 - numeros y booleans'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 05 - numeros y booleans',
            @InputVariables = N'{"max_tokens":512,"temperature":0.73,"stream":false,"retry_count":3}',
            @ExpectedOutput = N'[SEED] JSON con tipos mixtos.';
    END;

    FETCH NEXT FROM case_cursor
    INTO @SuiteId;
END;

CLOSE case_cursor;
DEALLOCATE case_cursor;
GO

 /* === Seed.TestRuns.sql === */
/*
  Cuatro ejecuciones de test por suite seed (modelo + temperatura + estado unicos por suite).
*/

DECLARE @SuiteId UNIQUEIDENTIFIER;
DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @PromptVersion INT;

DECLARE run_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    ts.[Id],
    ts.[PromptId],
    p.[Version]
FROM [dbo].[TestSuites] AS ts
INNER JOIN [dbo].[Prompts] AS p
    ON p.[Id] = ts.[PromptId]
WHERE
    ts.[Name] LIKE N'Suite seed v1:%'
    AND ts.[IsActive] = 1;

OPEN run_cursor;

FETCH NEXT FROM run_cursor
INTO @SuiteId, @PromptId, @PromptVersion;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gpt-4o'
          AND [Temperature] = 0.21
          AND [Status] = N'completed'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gpt-4o',
            @Temperature = 0.21,
            @Status = N'completed';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gpt-4o-mini'
          AND [Temperature] = 0.52
          AND [Status] = N'failed'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gpt-4o-mini',
            @Temperature = 0.52,
            @Status = N'failed';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'claude-3-5-sonnet'
          AND [Temperature] = 0.73
          AND [Status] = N'pending'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'claude-3-5-sonnet',
            @Temperature = 0.73,
            @Status = N'pending';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gemini-1.5-pro'
          AND [Temperature] = 0.94
          AND [Status] = N'running'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gemini-1.5-pro',
            @Temperature = 0.94,
            @Status = N'running';
    END;

    FETCH NEXT FROM run_cursor
    INTO @SuiteId, @PromptId, @PromptVersion;
END;

CLOSE run_cursor;
DEALLOCATE run_cursor;
GO

/* === Seed.AnalysisRuns.sql === */
/*
  Cuatro corridas de analisis por prompt activo (proveedores y latencias variadas, idempotente).
*/

DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @Snapshot NVARCHAR(MAX);
DECLARE @Temperature DECIMAL(4, 2);
DECLARE @MaxTokens INT;
DECLARE @TopP DECIMAL(4, 2);

DECLARE ar_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    [Id],
    [Content],
    [Temperature],
    [MaxTokens],
    [TopP]
FROM [dbo].[Prompts]
WHERE [IsActive] = 1;

OPEN ar_cursor;

FETCH NEXT FROM ar_cursor
INTO @PromptId, @Snapshot, @Temperature, @MaxTokens, @TopP;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'openai'
          AND [ModelId] = N'gpt-4o'
          AND [LatencyMs] = 237
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'openai',
            @ModelId = N'gpt-4o',
            @Input = N'[SEED] input openai caso 1',
            @Output = N'[SEED] output simulado openai gpt-4o',
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-OPENAI-GPT4O-237',
            @Status = N'Completed',
            @ErrorMessage = NULL,
            @LatencyMs = 237;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'anthropic'
          AND [ModelId] = N'claude-3-5-sonnet'
          AND [LatencyMs] = 890
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'anthropic',
            @ModelId = N'claude-3-5-sonnet',
            @Input = N'[SEED] input anthropic caso 2',
            @Output = NULL,
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-ANTHROPIC-890',
            @Status = N'Failed',
            @ErrorMessage = N'[SEED] Error simulado: rate limit',
            @LatencyMs = 890;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'google'
          AND [ModelId] = N'gemini-1.5-pro'
          AND [LatencyMs] = 1540
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'google',
            @ModelId = N'gemini-1.5-pro',
            @Input = N'[SEED] input google caso 3',
            @Output = N'[SEED] output simulado gemini',
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-GOOGLE-1540',
            @Status = N'Completed',
            @ErrorMessage = NULL,
            @LatencyMs = 1540;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'simulated'
          AND [ModelId] = N'simulated-perf'
          AND [LatencyMs] = 2890
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'simulated',
            @ModelId = N'simulated-perf',
            @Input = N'[SEED] input simulated caso 4',
            @Output = NULL,
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-SIM-PERF-2890',
            @Status = N'Running',
            @ErrorMessage = NULL,
            @LatencyMs = 2890;
    END;

    FETCH NEXT FROM ar_cursor
    INTO @PromptId, @Snapshot, @Temperature, @MaxTokens, @TopP;
END;

CLOSE ar_cursor;
DEALLOCATE ar_cursor;
