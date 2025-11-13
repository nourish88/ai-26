import { Suspense } from "react";
import { WizardClient } from "./_components/wizard-client";
import { fetchAllStrategies } from "./_services/chunking.service";
import { getEmbeddingModels } from "./_services/embeddings.service";
import { extractorEngineTypes } from "./_services/extractor.service";
import { getFileStores } from "./_services/file-store.service";
import { fetchExistingModels } from "./_services/llm.service";
import { getSearchEngines } from "./_services/search-engines.service";
import { getApplicationTypes, getMemoryTypes, getOutputTypes } from "./_services/types.service";
import { fetchAllMcpServers } from "./_services/mcp.service";

export default async function SetupWizardPage() {
  const existingLlms = await fetchExistingModels();
  const fetchChunkingStrategies = await fetchAllStrategies();
  const fetchFileStores = await getFileStores();
  const searchEngines = await getSearchEngines();
  const extractors = await extractorEngineTypes();
  const embeddings = await getEmbeddingModels();
  const applicationTypes = await getApplicationTypes();
  const memoryTypes = await getMemoryTypes();
  const outputTypes = await getOutputTypes();
  const mcpServers = await fetchAllMcpServers();

  return (
    <Suspense fallback={<div>Yükleniyor...</div>}>
      <WizardClient
        mcpServers={mcpServers.result.items}
        embeddings={embeddings.result.items}
        existingLlms={existingLlms.result.items}
        fetchChunkingStrategies={fetchChunkingStrategies.result.items}
        fileStores={fetchFileStores.data?.items}
        searchEngines={searchEngines.result.items}
        extractors={extractors.result.items}
        applicationTypes={applicationTypes.data?.items}
        memoryTypes={memoryTypes.data?.items}
        outputTypes={outputTypes.data?.items}
      />
    </Suspense>
  );
}
