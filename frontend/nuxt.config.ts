// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  imports: {
    dirs: ['stores']
  },
  modules: [
    '@nuxtjs/tailwindcss',
    'shadcn-nuxt',
    'nuxt-icon',
    '@nuxtjs/eslint-module',
    '@pinia/nuxt',
    '@pinia-plugin-persistedstate/nuxt'
  ],
  shadcn: {
    /**
     * Prefix for all the imported component
     */
    prefix: '',
    /**
     * Directory that the component lives in.
     * @default "./components/ui"
     */
    componentDir: './components/ui'
  },
  runtimeConfig: {
    public: {
      BASE_URL:
        process.env.NUXT_PUBLIC_BASE_URL ??
        'https://api.kozlowskip.pl/tinypath',
      MAX_GUEST_LINKS: process.env.NUXT_PUBLIC_MAX_GUEST_LINKS
    }
  },
  plugins: []
})
