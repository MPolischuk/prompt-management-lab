import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'Prompt Lab',
  tagline: 'Documentación técnica y funcional del laboratorio de gestión de prompts y pruebas',
  favicon: 'img/favicon.ico',

  future: {
    v4: true,
  },

  url: 'https://localhost',
  baseUrl: '/',

  organizationName: 'Dev-Marcos',
  projectName: 'prompt-management-lab',

  onBrokenLinks: 'throw',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          numberPrefixParser: false,
          editUrl:
            'https://github.com/Dev-Marcos/prompt-management-lab/edit/main/doc/business/docs/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: 'img/docusaurus-social-card.jpg',
    colorMode: {
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: 'Prompt Lab',
      logo: {
        alt: 'Prompt Lab',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'tutorialSidebar',
          position: 'left',
          label: 'Documentación',
        },
        {
          href: 'https://github.com/Dev-Marcos/prompt-management-lab',
          label: 'Código',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Documentación',
          items: [
            {
              label: 'Introducción',
              to: '/docs/intro',
            },
            {
              label: 'Arquitectura',
              to: '/docs/01-arquitectura/01-vision-general',
            },
            {
              label: 'API',
              to: '/docs/05-api-referencia/01-endpoints',
            },
          ],
        },
        {
          title: 'Repositorio',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/Dev-Marcos/prompt-management-lab',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Prompt Lab. Generado con Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
