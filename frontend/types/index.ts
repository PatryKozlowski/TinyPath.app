export type LoginForm = {
  email: string
  password: string
}

export type LoginResponse = {
  token: string
  refreshToken: string
}

export type RegisterForm = {
  email: string
  password: string
  repeatPassword: string
}

export type ResetPasswordForm = {
  password: string
  repeatPassword: string
}

export type Hidden = {
  password: boolean
  repeatPassword: boolean
}

export type LoadedUserDataResponse = {
  isSubscribed: boolean
  isAdmin: boolean
  email: string
}

export type LogoutResponse = {
  message: string
}

export type CreateLinkForm = {
  url: string
  title?: string
}

export type CreateLinkResponse = {
  link: string
}

export type CreateCustomLinkForm = CreateLinkForm & {
  customCode: string
}

export type AuthUser = {
  email: string
  token: string
  isSubscribed: boolean
  isAdmin: boolean
}

export type GuestData = {
  links: number
  blocked: boolean
  blockedUntil: Date | null
}

export type GuestEmailForm = {
  email: string
}

export type CheckoutForm = {
  priceCode: string
}

export type CheckoutResponse = {
  link: string
}

export type LinksResponse = {
  urls: [
    {
      id: string
      url: string
      title: string
      active: boolean
      isCustom: boolean
    }
  ]
  totalPages: number
  totalRecords: number
}

export type LinkResponse = {
  id: string
  url: string
  title: string
  active: boolean
  isCustom: boolean
  orginalUrl: string
  created: Date
  updated: Date
  code: string
}

export type DeleteLinkResponse = {
  success: boolean
}

export type EditLinkForm = {
  title?: string
  active?: boolean
}
