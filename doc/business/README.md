# Documentación Prompt Lab (Docusaurus)

Sitio estático con la documentación técnica y funcional del repositorio **prompt-management-lab**.

## Requisitos

- Node.js **≥ 20**

## Instalación

```bash
npm install
```

## Desarrollo

```bash
npm start
```

## Build de producción

```bash
npm run build
```

Los archivos generados quedan en la carpeta `build/`.

## Contenido

Los artículos están en `docs/`. La entrada principal es [Introducción](./docs/intro.md).

## Notas

- `numberPrefixParser: false` mantiene en la URL los prefijos numéricos de carpetas y archivos (`01-arquitectura`, etc.).
- El repositorio remoto en `docusaurus.config.ts` (`Dev-Marcos/prompt-management-lab`) y el `editUrl` pueden ajustarse si el fork o la organización difieren.
