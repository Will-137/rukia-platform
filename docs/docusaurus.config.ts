import { themes as prismThemes } from 'prism-react-renderer';
import type { Config } from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

const config: Config = {
  title: 'RUKIA',
  tagline: 'Documentação RUKIA',
  favicon: 'img/favicon.ico',

  future: {
    v4: true,
  },


  // 🔥 GitHub Pages config correta
  url: 'https://will-137.github.io',
  baseUrl: '/rukia-platform/',

  organizationName: 'Will-137',
  projectName: 'rukia-platform',

  deploymentBranch: 'gh-pages',

  //onBrokenLinks: 'throw',
  onBrokenLinks: 'warn',
  markdown: {
    hooks: {
      onBrokenMarkdownLinks: 'warn',
    },
  },

  i18n: {
    defaultLocale: 'pt-BR',
    locales: ['pt-BR'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: require.resolve('./sidebars.ts'),
          routeBasePath: 'docs',
          editUrl:
            'https://github.com/Will-137/rukia-platform/tree/main/docs/',
        },
        blog: {
          showReadingTime: true,
          editUrl:
            'https://github.com/Will-137/rukia-platform/tree/main/blog/',
          onInlineTags: 'warn',
          onInlineAuthors: 'warn',
          onUntruncatedBlogPosts: 'warn',
        },
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    colorMode: {
      respectPrefersColorScheme: true,
      defaultMode: 'dark',
      disableSwitch: false,
    },

    navbar: {
      title: 'RUKIA',
      logo: {
        alt: 'RUKIA',
        src: 'img/logo.svg',
      },
      items: [
        //{ to: '/', label: 'Home', position: 'left' },
        { to: '/docs/intro', label: 'Docs', position: 'left' },
        { to: '/docs/tutorials', label: 'Tutoriais', position: 'left' },
        { to: '/docs/api', label: 'API', position: 'left' },
		{ to: '/iris', label: 'IRIS' },
        //{ to: '/docs/agente-iris', label: 'Agente IA', position: 'left' },
        { to: '/blog', label: 'Blog', position: 'left' },
        {
          href: 'https://github.com/Will-137/rukia-platform',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },

    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            { label: 'Introdução', to: '/docs/intro' },
            { label: 'Agente IA (iris)', to: '/docs/agente-iris' },
          ],
        },
        {
          title: 'Projeto',
          items: [
            { label: 'Blog', to: '/blog' },
            {
              label: 'GitHub',
              href: 'https://github.com/Will-137/rukia-platform',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} RUKIA`,
    },

    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
  } satisfies Preset.ThemeConfig,
};

export default config;