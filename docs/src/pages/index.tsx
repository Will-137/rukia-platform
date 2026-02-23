import React from "react";
import clsx from "clsx";
import Layout from "@theme/Layout";
import Link from "@docusaurus/Link";
import useDocusaurusContext from "@docusaurus/useDocusaurusContext";
import styles from "/src/css/index.module.css";

type FeatureItem = {
  title: string;
  description: React.ReactNode;
  icon: string; // FontAwesome class or emoji fallback
  href?: string;
};

const FeatureList: FeatureItem[] = [
  {
    title: "Governança Real",
    description: (
      <>
        Fonte única da verdade por domínio, regras explícitas e decisões congeladas.
        O sistema não depende de “memória do time”.
      </>
    ),
    icon: "🧭",
    href: "/docs/governanca/visao-geral",
  },
  {
    title: "Eventos criam verdade",
    description: (
      <>
        Nada muda por edição. Cada avanço é um evento auditável. Histórico imutável e
        rastreabilidade de ponta a ponta.
      </>
    ),
    icon: "🧾",
    href: "/docs/arquitetura/visao-geral",
  },
  {
    title: "Construído para Confecção",
    description: (
      <>
        PLM, PCP, Estoque, Compras e Oficinas refletindo o chão de fábrica real —
        sem ERP genérico e sem atalhos.
      </>
    ),
    icon: "🏭",
    href: "/docs/dominios/",
  },
  {
    title: "IRIS integrada ao fluxo",
    description: (
      <>
        Assistente contextual por perfil de usuário. Orienta, alerta e guia no momento
        exato do erro — sem quebrar governança.
      </>
    ),
    icon: "🧠",
    href: "/IRIS/overview",
  },
];

type QuickLink = { title: string; description: string; href: string; icon: string };

const QuickLinks: QuickLink[] = [
  { title: "Introdução", description: "Comece pelo essencial do projeto", href: "/docs/intro", icon: "🚀" },
  { title: "Governança (SAKURA)", description: "Regras, SSOT, congelamentos", href: "/docs/governanca/visao-geral", icon: "🛡️" },
  { title: "Arquitetura", description: "Visão macro e fluxos canônicos", href: "/docs/arquitetura/visao-geral", icon: "🏗️" },
  { title: "Domínios V1", description: "PLM, PCP, Estoque, Compras…", href: "/docs/dominios/", icon: "🧩" },
  { title: "IRIS", description: "Agente operacional por perfil", href: "/IRIS/overview", icon: "🧠" },
  { title: "Roadmap", description: "Próximas entregas e evolução", href: "/roadmap", icon: "🗺️" },
];

function FeatureCard({ title, description, icon, href }: FeatureItem) {
  const content = (
    <div className={clsx("card", styles.card, styles.cardHover)}>
      <div className={clsx("card__body", styles.cardBody)}>
        <div className={styles.cardIcon} aria-hidden="true">{icon}</div>
        <h3 className={styles.cardTitle}>{title}</h3>
        <p className={styles.cardDesc}>{description}</p>
        <div className={styles.cardFooter}>
          <span className={styles.cardLinkHint}>Saiba mais →</span>
        </div>
      </div>
    </div>
  );

  if (!href) return content;

  return (
    <Link className={styles.cardLinkWrapper} to={href}>
      {content}
    </Link>
  );
}

function QuickLinkCard({ title, description, href, icon }: QuickLink) {
  return (
    <Link className={styles.quickLinkWrapper} to={href}>
      <div className={clsx("card", styles.quickCard, styles.cardHover)}>
        <div className={clsx("card__body", styles.quickCardBody)}>
          <div className={styles.quickIcon} aria-hidden="true">{icon}</div>
          <div className={styles.quickContent}>
            <div className={styles.quickTitle}>{title}</div>
            <div className={styles.quickDesc}>{description}</div>
          </div>
        </div>
      </div>
    </Link>
  );
}

function HomeHero() {
  return (
    <header className={clsx("hero", styles.hero)}>
      <div className={clsx("container", styles.heroContainer)}>
        <div className={styles.heroInner}>
			<h1 className={styles.heroTitle}>RUKIA</h1>
			<h2 className={styles.heroSubtitleAlt}>
			  R.ecursos U.nificados Kom Inteligência Avançada
			</h2>

          <p className={styles.heroSubtitle}>
            Uma plataforma ERP SaaS completa, inteligente, modular e escalável, 
            projetada para evoluir continuamente junto com o crescimento da sua Empresa.
          </p>

          <div className={styles.heroButtons}>
            <Link className={clsx("button button--primary button--lg", styles.ctaPrimary)} to="/docs/intro">
              Começar agora
            </Link>
            <Link className={clsx("button button--secondary button--lg", styles.ctaSecondary)} to="/docs/arquitetura/visao-geral">
              Ver Arquitetura
            </Link>
          </div>

          <div className={styles.heroTrustLine}>
            V1 com fases congeladas e contratos canônicos para execução sem ambiguidade.
          </div>
        </div>
      </div>
    </header>
  );
}

function IRISSpotlight() {
  return (
    <section className={styles.sectionAlt}>
      <div className="container">
        <div className={styles.split}>
          <div className={styles.splitLeft}>
            <h2 className={styles.sectionTitle}>IRIS não é um chatbot. Ela é a guia do sistema.</h2>
            <p className={styles.sectionText}>
              IRIS opera por perfil de usuário (PCP, PLM, Almoxarifado, Direção). Ela explica por que algo
              está travado, o que falta para avançar e onde a OP está — sempre respeitando permissões e a
              fonte da verdade do domínio.
            </p>
            <div className={styles.sectionButtons}>
              <Link className={clsx("button button--primary", styles.ctaPrimary)} to="/IRIS/overview">
                Conhecer a IRIS
              </Link>
            </div>
          </div>

          <div className={styles.splitRight} aria-hidden="true">
            <div className={styles.irisPanel}>
              <div className={styles.irisRow}><span className={styles.irisDot} /> IRIS, isso travou…</div>
              <div className={styles.irisRow}><span className={styles.irisDot} /> IRIS, não consigo preencher. Por quê?</div>
              <div className={styles.irisRow}><span className={styles.irisDot} /> IRIS, a OP está em qual setor?</div>
              <div className={styles.irisHint}>
                Guia contextual • Alertas • Passo a passo por perfil
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function MaturitySection() {
  return (
    <section className={styles.section}>
      <div className="container">
        <h2 className={styles.sectionTitle}>Maturidade e confiança</h2>
        <p className={styles.sectionText}>
          A RUKIA é documentada com governança explícita: decisões congeladas, contratos canônicos e rastreabilidade
          orientada por eventos.
        </p>

        <div className={styles.maturityGrid}>
          <div className={styles.maturityItem}>
            <div className={styles.maturityKicker}>Fases V1</div>
            <div className={styles.maturityValue}>com escopo congelado</div>
          </div>
          <div className={styles.maturityItem}>
            <div className={styles.maturityKicker}>Contratos</div>
            <div className={styles.maturityValue}>canônicos por domínio</div>
          </div>
          <div className={styles.maturityItem}>
            <div className={styles.maturityKicker}>Fluxos</div>
            <div className={styles.maturityValue}>Produto → Pedido → OP → Estoque</div>
          </div>
          <div className={styles.maturityItem}>
            <div className={styles.maturityKicker}>Checkpoints</div>
            <div className={styles.maturityValue}>packs versionados e auditáveis</div>
          </div>
        </div>
      </div>
    </section>
  );
}

function AudienceSection() {
  return (
    <section className={styles.sectionAlt}>
      <div className="container">
        <h2 className={styles.sectionTitle}>A RUKIA foi projetada para:</h2>

        <div className={styles.audienceGrid}>
          <div className={clsx("card", styles.card)}>
            <div className="card__body">
              <h3 className={styles.cardTitle}>Diretoria</h3>
              <p className={styles.cardDesc}>
                Previsibilidade, controle e visão clara do que está em execução — sem microgestão e sem “achismo”.
              </p>
            </div>
          </div>

          <div className={clsx("card", styles.card)}>
            <div className="card__body">
              <h3 className={styles.cardTitle}>Operação</h3>
              <p className={styles.cardDesc}>
                Fluxos simples e rastreáveis. Menos retrabalho, menos erro de comunicação e mais velocidade no dia a dia.
              </p>
            </div>
          </div>

          <div className={clsx("card", styles.card)}>
            <div className="card__body">
              <h3 className={styles.cardTitle}>Desenvolvimento</h3>
              <p className={styles.cardDesc}>
                Contratos claros, evolução segura e documentação canônica — sem regras escondidas e sem dependência de contexto.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function FeaturesSection() {
  return (
    <section className={styles.section}>
      <div className="container">
        <h2 className={styles.sectionTitle}>Recursos da Plataforma</h2>
        <p className={styles.sectionText}>
          Uma base sólida para operação industrial com clareza de domínio, governança e execução.
        </p>

        <div className={styles.grid}>
          {FeatureList.map((props, idx) => (
            <FeatureCard key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}

function QuickLinksSection() {
  return (
    <section className={styles.section}>
      <div className="container">
        <h2 className={styles.sectionTitle}>Acesso Rápido</h2>
        <p className={styles.sectionText}>Links principais para navegar a documentação com velocidade.</p>

        <div className={styles.quickGrid}>
          {QuickLinks.map((q, idx) => (
            <QuickLinkCard key={idx} {...q} />
          ))}
        </div>
      </div>
    </section>
  );
}

export default function Home(): JSX.Element {
  const { siteConfig } = useDocusaurusContext();

  return (
    <Layout
      title={siteConfig.title}
      description="RUKIA — Plataforma ERP SaaS completa, inteligente, modular e escalável."
    >
      <main>
        <HomeHero />
        <FeaturesSection />
        <IRISSpotlight />
        <MaturitySection />
        <AudienceSection />
        <QuickLinksSection />
      </main>
    </Layout>
  );
}
