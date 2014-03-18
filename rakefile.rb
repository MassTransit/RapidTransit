COPYRIGHT = "Copyright 2014 Chris Patterson, All rights reserved."

require File.dirname(__FILE__) + "/build_support/BuildUtils.rb"
require File.dirname(__FILE__) + "/build_support/util.rb"
include FileTest
require 'albacore'
require File.dirname(__FILE__) + "/build_support/versioning.rb"

PRODUCT = 'RapidTransit'
CLR_TOOLS_VERSION = 'v4.0.30319'
OUTPUT_PATH = 'bin/Release'

props = {
  :src => File.expand_path("src"),
  :output => File.expand_path("build_output"),
  :artifacts => File.expand_path("build_artifacts"),
  :projects => ["RapidTransit"]
}

desc "Cleans, compiles, il-merges, unit tests, prepares examples, packages zip"
task :all => [:default, :package]

desc "**Default**, compiles and runs tests"
task :default => [:clean, :nuget_restore, :compile, :package]

desc "Update the common version information for the build. You can call this task without building."
assemblyinfo :global_version do |asm|
  # Assembly file config
  asm.product_name = PRODUCT
  asm.description = "RapidTransit, a quick library to get started building autonomous services using MassTransit"
  asm.version = FORMAL_VERSION
  asm.file_version = FORMAL_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}",
	:ComVisibleAttribute => false,
	:CLSCompliantAttribute => true
  asm.copyright = COPYRIGHT
  asm.output_file = 'src/SolutionVersion.cs'
  asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices"
end

desc "Prepares the working directory for a new build"
task :clean do
	FileUtils.rm_rf props[:output]
	waitfor { !exists?(props[:output]) }

	FileUtils.rm_rf props[:artifacts]
	waitfor { !exists?(props[:artifacts]) }

	Dir.mkdir props[:output]
	Dir.mkdir props[:artifacts]
end

desc "Cleans, versions, compiles the application and generates build_output/."
task :compile => [:versioning, :global_version, :build] do
	copyOutputFiles File.join(props[:src], "RapidTransit.Core/bin/Release"), "RapidTransit.Core.{dll,pdb,xml}", File.join(props[:output], 'net-4.5')
  copyOutputFiles File.join(props[:src], "RapidTransit.Integration/bin/Release"), "RapidTransit.Integration.{dll,pdb,xml}", File.join(props[:output], 'net-4.5')
  copyOutputFiles File.join(props[:src], "RapidTransit.Integration.Web/bin/Release"), "RapidTransit.Integration.Web.{dll,pdb,xml}", File.join(props[:output], 'net-4.5')
  copyOutputFiles File.join(props[:src], "RapidTransit.Integration.Services/bin/Release"), "RapidTransit.Integration.Services.{dll,pdb,xml}", File.join(props[:output], 'net-4.5')
end

desc "Only compiles the application."
msbuild :build do |msb|
	msb.properties :Configuration => "Release",
		:Platform => 'Any CPU'
	msb.use :net4
	msb.targets :Clean, :Build
  msb.properties[:SignAssembly] = 'true'
  msb.properties[:AssemblyOriginatorKeyFile] = props[:keyfile]
	msb.solution = 'src/RapidTransit.sln'
end

def copyOutputFiles(fromDir, filePattern, outDir)
	FileUtils.mkdir_p outDir unless exists?(outDir)
	Dir.glob(File.join(fromDir, filePattern)){|file|
		copy(file, outDir) if File.file?(file)
	}
end

desc "Runs unit tests"
nunit :tests => [:compile] do |nunit|

          nunit.command = File.join('src', 'packages','NUnit.Runners.2.6.3', 'tools', 'nunit-console.exe')
          nunit.options = "/framework=#{CLR_TOOLS_VERSION}", '/nothread', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'nunit-test-results.xml')}\""
          nunit.assemblies = FileList[File.join(props[:src], "RapidTransit.Tests/bin/Release", "RapidTransit.Tests.dll")]
end

task :package => [:nuget, :zip_output]

desc "ZIPs up the build results."
zip :zip_output => [:versioning] do |zip|
	zip.directories_to_zip = [props[:output]]
	zip.output_file = "RapidTransit-#{NUGET_VERSION}.zip"
	zip.output_path = props[:artifacts]
end

desc "Restore missing NuGet packages"
task :nuget_restore do
  sh "src/.nuget/nuget.exe restore #{props[:src]}/RapidTransit.sln"
end

desc "Builds the nuget package"
task :nuget => [:versioning, :create_nuspec] do
	sh "src/.nuget/nuget.exe pack #{props[:artifacts]}/RapidTransit.Core.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
  sh "src/.nuget/nuget.exe pack #{props[:artifacts]}/RapidTransit.Integration.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
  sh "src/.nuget/nuget.exe pack #{props[:artifacts]}/RapidTransit.Integration.Web.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
	sh "src/.nuget/nuget.exe pack #{props[:artifacts]}/RapidTransit.Integration.Services.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
end

task :create_nuspec => [:_nuspec]

nuspec :_nuspec do |nuspec|
  nuspec.id = 'RapidTransit.Core'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'RapidTransit, a quick library to get started building autonomous services using MassTransit'
  nuspec.title = 'RapidTransit.Core'
  nuspec.projectUrl = 'http://github.com/MassTransit/RapidTransit'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency 'Automatonymous', "1.2.5"
  nuspec.dependency 'Automatonymous.MassTransit', "1.2.5"
  nuspec.dependency 'Magnum', "2.1.2"
  nuspec.dependency 'MassTransit', "2.9.5"
  nuspec.dependency 'MassTransit.Courier', "2.9.5"
  nuspec.dependency 'MassTransit.Scheduling', "1.2.5"
  nuspec.dependency 'Newtonsoft.Json', "5.0.8"
  nuspec.dependency 'RabbitMQ.Client', "3.2.1"
  nuspec.dependency 'Taskell', "0.1.2"
  nuspec.output_file = File.join(props[:artifacts], 'RapidTransit.Core.nuspec')
  add_files props[:output], 'RapidTransit.Core.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "RapidTransit.Core\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :_nuspec do |nuspec|
  nuspec.id = 'RapidTransit.Integration'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'RapidTransit, a quick library to get started building autonomous services using MassTransit'
  nuspec.title = 'RapidTransit.Integration'
  nuspec.projectUrl = 'http://github.com/MassTransit/RapidTransit'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "RapidTransit.Core", NUGET_VERSION
  nuspec.dependency 'Autofac', "3.3.0"
  nuspec.dependency 'Automatonymous', "1.2.5"
  nuspec.dependency 'Automatonymous.MassTransit', "1.2.5"
  nuspec.dependency 'Magnum', "2.1.2"
  nuspec.dependency 'MassTransit', "2.9.5"
  nuspec.dependency 'MassTransit.Autofac', "2.9.5"
  nuspec.dependency 'MassTransit.Courier', "2.9.5"
  nuspec.dependency 'MassTransit.RabbitMQ', "2.9.5"
  nuspec.dependency 'MassTransit.Scheduling', "1.2.5"
  nuspec.dependency 'Newtonsoft.Json', "5.0.8"
  nuspec.dependency 'RabbitMQ.Client', "3.2.1"
  nuspec.dependency 'Taskell', "0.1.2"
  nuspec.output_file = File.join(props[:artifacts], 'RapidTransit.Integration.nuspec')
  add_files props[:output], 'RapidTransit.Integration.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "RapidTransit.Integration\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :_nuspec do |nuspec|
  nuspec.id = 'RapidTransit.Integration.Web'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'RapidTransit, a quick library to get started building autonomous services using MassTransit'
  nuspec.title = 'RapidTransit.Integration.Web'
  nuspec.projectUrl = 'http://github.com/MassTransit/RapidTransit'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "RapidTransit.Core", NUGET_VERSION
  nuspec.dependency "RapidTransit.Integration", NUGET_VERSION
  nuspec.dependency 'Autofac', "3.3.0"
  nuspec.dependency 'Automatonymous', "1.2.5"
  nuspec.dependency 'Automatonymous.MassTransit', "1.2.5"
  nuspec.dependency 'Magnum', "2.1.2"
  nuspec.dependency 'MassTransit', "2.9.5"
  nuspec.dependency 'MassTransit.Autofac', "2.9.5"
  nuspec.dependency 'MassTransit.Courier', "2.9.5"
  nuspec.dependency 'MassTransit.RabbitMQ', "2.9.5"
  nuspec.dependency 'MassTransit.Scheduling', "1.2.5"
  nuspec.dependency 'Newtonsoft.Json', "5.0.8"
  nuspec.dependency 'RabbitMQ.Client', "3.2.1"
  nuspec.dependency 'Taskell', "0.1.2"
  nuspec.output_file = File.join(props[:artifacts], 'RapidTransit.Integration.Web.nuspec')
  add_files props[:output], 'RapidTransit.Integration.Web.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "RapidTransit.Integration.Web\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :_nuspec do |nuspec|
  nuspec.id = 'RapidTransit.Integration.Services'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'RapidTransit, a quick library to get started building autonomous services using MassTransit'
  nuspec.title = 'RapidTransit.Integration.Services'
  nuspec.projectUrl = 'http://github.com/MassTransit/RapidTransit'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "RapidTransit.Core", NUGET_VERSION
  nuspec.dependency "RapidTransit.Integration", NUGET_VERSION
  nuspec.dependency 'Autofac', "3.3.0"
  nuspec.dependency 'Automatonymous', "1.2.5"
  nuspec.dependency 'Automatonymous.MassTransit', "1.2.5"
  nuspec.dependency 'Common.Logging', '2.2.0'
  nuspec.dependency 'Common.Logging.Core', '2.2.0'
  nuspec.dependency 'Magnum', "2.1.2"
  nuspec.dependency 'MassTransit', "2.9.5"
  nuspec.dependency 'MassTransit.Autofac', "2.9.5"
  nuspec.dependency 'MassTransit.Courier', "2.9.5"
  nuspec.dependency 'MassTransit.QuartzIntegration', '1.2.5'
  nuspec.dependency 'MassTransit.RabbitMQ', "2.9.5"
  nuspec.dependency 'MassTransit.Scheduling', '1.2.5'
  nuspec.dependency 'Newtonsoft.Json', "5.0.8"
  nuspec.dependency 'Quartz', '2.2.2'
  nuspec.dependency 'RabbitMQ.Client', "3.2.1"
  nuspec.dependency 'Taskell', "0.1.2"
  nuspec.dependency 'Topshelf', '3.1.3'
  nuspec.output_file = File.join(props[:artifacts], 'RapidTransit.Integration.Services.nuspec')
  add_files props[:output], 'RapidTransit.Integration.Services.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "RapidTransit.Integration.Services\\**\\*.cs").gsub("/","\\"), "src")
end

def project_outputs(props)
	props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.dll" }.
		concat( props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.exe" } ).
		find_all{ |path| exists?(path) }
end

def get_commit_hash_and_date
	begin
		commit = `git log -1 --pretty=format:%H`
		git_date = `git log -1 --date=iso --pretty=format:%ad`
		commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
	rescue
		commit = "git unavailable"
	end

	[commit, commit_date]
end

def add_files stage, what_dlls, nuspec
  [['net40', 'net-4.0'], ['net45', 'net-4.5']].each{|fw|
    takeFrom = File.join(stage, fw[1], what_dlls)
    Dir.glob(takeFrom).each do |f|
      nuspec.file(f.gsub("/", "\\"), "lib\\#{fw[0]}")
    end
  }
end

def waitfor(&block)
	checks = 0

	until block.call || checks >10
		sleep 0.5
		checks += 1
	end

	raise 'Waitfor timeout expired. Make sure that you aren\'t running something from the build output folders, or that you have browsed to it through Explorer.' if checks > 10
end
